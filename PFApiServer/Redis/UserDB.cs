using CloudStructures.Structures;
using CloudStructures;
using Common.Enum;
using static PFApiServer.LogManager;
using ZLogger;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PFApiServer.Redis
{
    public class UserDB : IUserDB
    {
#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
        public RedisConnection _redisConn;
#pragma warning restore CS8618 // 생성자를 종료할 때 null을 허용하지 않는 필드에 null이 아닌 값을 포함해야 합니다. null 허용으로 선언해 보세요.
        private static readonly ILogger<UserDB> s_logger = GetLogger<UserDB>();

        public void Init(string address)
        {
            RedisConfig config = new("default", address);
            _redisConn = new RedisConnection(config);

            s_logger.ZLogDebug($"userDbAddress:{address}");
        }


        public async Task<ErrorCode> RegistUserAsync(string id, string authToken, long uid)
        {
            string key = RedisKeyMaker.MakeIDKey(id);
            ErrorCode result = ErrorCode.None;

            RdbAuthUserData user = new()
            {
                Id = id,
                AuthToken = authToken,
                Uid = uid,
                State = UserState.Default.ToString()
            };

            try
            {
                RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
                if (await redis.SetAsync(user, LoginTimeSpan()) == false)
                {
                    s_logger.ZLogError(EventIdDic[EventType.LoginAddRedis],
                        $"Id:{id}, AuthToken:{authToken},ErrorMessage:UserBasicAuth, RedisString set Error");
                    result = ErrorCode.LoginFailAddRedis;
                    return result;
                }
            }
            catch
            {
                s_logger.ZLogError(EventIdDic[EventType.LoginAddRedis],
                    $"Id:{id},AuthToken:{authToken},ErrorMessage:Redis Connection Error");
                result = ErrorCode.LoginFailAddRedis;
                return result;
            }

            return result;
        }

        public async Task<ErrorCode> CheckUserAuthAsync(string id, string authToken)
        {
            string key = RedisKeyMaker.MakeIDKey(id);
            ErrorCode result = ErrorCode.None;

            try
            {
                RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
                RedisResult<RdbAuthUserData> user = await redis.GetAsync();

                if (!user.HasValue)
                {
                    s_logger.ZLogError(EventIdDic[EventType.Login],
                        $"RedisDb.CheckUserAuthAsync: Id = {id}, AuthToken = {authToken}, ErrorMessage:ID does Not Exist");
                    result = ErrorCode.CheckAuthFailNotExist;
                    return result;
                }

                if (user.Value.Id != id || user.Value.AuthToken != authToken)
                {
                    s_logger.ZLogError(EventIdDic[EventType.Login],
                        $"RedisDb.CheckUserAuthAsync: Id = {id}, AuthToken = {authToken}, ErrorMessage = Wrong ID or Auth Token");
                    result = ErrorCode.CheckAuthFailNotMatch;
                    return result;
                }
            }
            catch
            {
                s_logger.ZLogError(EventIdDic[EventType.Login],
                    $"RedisDb.CheckUserAuthAsync: Id = {id}, AuthToken = {authToken}, ErrorMessage:Redis Connection Error");
                result = ErrorCode.CheckAuthFailException;
                return result;
            }


            return result;
        }

        public async Task<bool> SetUserStateAsync(RdbAuthUserData user, UserState userState)
        {
            string uid = RedisKeyMaker.MakeIDKey(user.Id);
            try
            {
                RedisString<RdbAuthUserData> redis = new(_redisConn, uid, null);

                user.State = userState.ToString();

                return await redis.SetAsync(user) != false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool, RdbAuthUserData?)> GetUserAsync(string id)
        {
            string uid = RedisKeyMaker.MakeIDKey(id);

            try
            {
                RedisString<RdbAuthUserData?> redis = new(_redisConn, uid, null);
                RedisResult<RdbAuthUserData?> user = await redis.GetAsync();
                if (!user.HasValue)
                {
                    s_logger.ZLogError(
                        $"RedisDb.UserStartCheckAsync: UID = {uid}, ErrorMessage = Not Assigned User, RedisString get Error");
                    return (false, null);
                }

                return (true, user.Value);
            }
            catch
            {
                s_logger.ZLogError($"UID:{uid},ErrorMessage:ID does Not Exist");
                return (false, null);
            }
        }

        public async Task<bool> SetUserReqLockAsync(string key)
        {
            try
            {
                RedisString<RdbAuthUserData> redis = new(_redisConn, key, NxKeyTimeSpan());
                if (await redis.SetAsync(new RdbAuthUserData
                {
                    // emtpy value
                }, NxKeyTimeSpan(), StackExchange.Redis.When.NotExists) == false)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DelUserReqLockAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            try
            {
                RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
                bool redisResult = await redis.DeleteAsync();
                return redisResult;
            }
            catch
            {
                return false;
            }
        }


        public TimeSpan LoginTimeSpan()
        {
            return TimeSpan.FromMinutes(RedisKeyExpireTime.LoginKeyExpireMin);
        }

        public TimeSpan TicketKeyTimeSpan()
        {
            return TimeSpan.FromSeconds(RedisKeyExpireTime.TicketKeyExpireSecond);
        }

        public TimeSpan NxKeyTimeSpan()
        {
            return TimeSpan.FromSeconds(RedisKeyExpireTime.NxKeyExpireSecond);
        }
    }
}
