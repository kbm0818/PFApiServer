using Common.Enum;
using Microsoft.AspNetCore.Http;
using PFApiServer.Redis;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PFApiServer.Middleware
{
    public class CheckUserAuthAndLoadUserData
    {
        private readonly IUserDB _redis;
        private readonly RequestDelegate _next;

        public CheckUserAuthAndLoadUserData(RequestDelegate next, IUserDB redis)
        {
            _redis = redis;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? formString = context.Request.Path.Value;
            if (string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(formString, "/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await _next(context);

                return;
            }

            context.Request.EnableBuffering();

            string userLockKey = "";

            using (StreamReader reader = new(context.Request.Body, Encoding.UTF8, true, 4096, true))
            {
                string bodyStr = await reader.ReadToEndAsync();

                // body String에 어떤 문자열도 없을때
                if (await IsNullBodyDataThenSendError(context, bodyStr))
                {
                    return;
                }


                JsonDocument document = JsonDocument.Parse(bodyStr);

                if (IsInvalidJsonFormatThenSendError(context, document, out string email, out string AuthToken))
                {
                    return;
                }

                (bool isOk, RdbAuthUserData? userInfo) = await _redis.GetUserAsync(email);
                if (isOk == false)
                {
                    return;
                }

                if(userInfo == null) 
                { 
                    return; 
                }

                if (await IsInvalidUserAuthTokenThenSendError(context, userInfo, AuthToken))
                {
                    return;
                }

                userLockKey = RedisKeyMaker.MakeUserLockKey(userInfo.Id);
                if (await SetLockAndIsFailThenSendError(context, userLockKey))
                {
                    return;
                }

                context.Items[nameof(RdbAuthUserData)] = userInfo;
            }

            context.Request.Body.Position = 0;

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            // 트랜잭션 해제(Redis 동기화 해제)
            _ = await _redis.DelUserReqLockAsync(userLockKey);
        }

        private async Task<bool> SetLockAndIsFailThenSendError(HttpContext context, string AuthToken)
        {
            if (await _redis.SetUserReqLockAsync(AuthToken))
            {
                return false;
            }


            string errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailSetNx
            });
            byte[] bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            return true;
        }

        private static async Task<bool> IsInvalidUserAuthTokenThenSendError(HttpContext context, RdbAuthUserData userInfo, string authToken)
        {
            if (string.CompareOrdinal(userInfo.AuthToken, authToken) == 0)
            {
                return false;
            }


            string errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailWrongAuthToken
            });
            byte[] bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return true;
        }

        private bool IsInvalidJsonFormatThenSendError(HttpContext context, JsonDocument document, out string id,
            out string authToken)
        {
            try
            {
                id = document.RootElement.GetProperty("Id").GetString() ?? throw new ArgumentNullException(paramName: "Id", message: "Id cannot be null");
                authToken = document.RootElement.GetProperty("AuthToken").GetString() ?? throw new ArgumentNullException(paramName: "AuthToken", message: "AuthToken cannot be null"); ;
                return false;
            }
            catch
            {
                id = ""; authToken = "";

                string errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
                {
                    result = ErrorCode.AuthTokenFailWrongKeyword
                });

                byte[] bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
                context.Response.Body.Write(bytes, 0, bytes.Length);

                return true;
            }
        }

        private async Task<bool> IsNullBodyDataThenSendError(HttpContext context, string bodyStr)
        {
            if (string.IsNullOrEmpty(bodyStr) == false)
            {
                return false;
            }

            string errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.InValidRequestHttpBody
            });
            byte[] bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return true;
        }


        public class MiddlewareResponse
        {
            public ErrorCode result { get; set; }
        }
    }
}
