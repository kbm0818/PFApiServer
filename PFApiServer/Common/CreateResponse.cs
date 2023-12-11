using Newtonsoft.Json;
using PFApiServer.Common.Enums;
using StackExchange.Redis;

namespace PFApiServer.Common
{
    /// <summary>
    /// 응답용 클래스
    /// Static 함수인 CreateJsonResponse 호출하여 생성한다
    /// </summary>
    public class CreateResponse(bool isSucess, object? returnValue, string msg, Dictionary<string, object>? addedProperties = null)
    {
        public bool IsSucess { get; set; } = isSucess;
        public object? ReturnValue { get; set; } = returnValue;
        public string Msg { get; set; } = msg;
        public Dictionary<string, object> AddedProperties { get; set; } = addedProperties ?? [];

        public static CreateResponse CreateJsonResponse(IConnectionMultiplexer redis, bool isValid, object? returnValue, string msg, object request, long uid, string methodName, DateTime processStartTime, Dictionary<string, object>? addedProperties = null, object? response = null, bool isLog = true)
        {
            addedProperties ??= new Dictionary<string, object>();
            CreateResponse res = new(isValid, returnValue, msg, addedProperties);

            // 세션 등록 (레디스에)
            redis.GetDatabase((int)EnumRedisDB.Session).StringSetAsync(uid.ToString(), "", new TimeSpan(0, 0, 0, 90));

            // 레디스에 피킷 넘버 증가
            var packetRedis = redis.GetDatabase((int)EnumRedisDB.Packet);
            packetRedis.StringIncrement(uid.ToString());

            // 레디스에 마지막 패킷 등록
            packetRedis.StringSetAsync(uid.ToString() + "_PACKET", JsonConvert.SerializeObject(res));

            return res;
        }
    }
}
