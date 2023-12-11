using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using PFApiServer.Common.Enums;
using PFApiServer.Common;
using StackExchange.Redis;
using Microsoft.IdentityModel.Tokens;

namespace PFApiServer.Middleware
{
    public class TokenValidateMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConnectionMultiplexer _redis = redis;
        private static readonly List<string> _excludeList =
        [
                "/Game/GameUser/Login",
                "/Game/GameUser/CheckVersion"
        ];
        
        public async Task Invoke(HttpContext httpContext)
        {
            var redis = _redis.GetDatabase((int)EnumRedisDB.Token);
            httpContext.Request.coo
            httpContext.Request.Headers.TryGetValue("uid", out StringValues uid);

            if (httpContext.Request.Path.Value is not null)
            {
                if (_excludeList.Contains(httpContext.Request.Path.Value) is false)
                {
                    if (uid.IsNullOrEmpty() is false)
                    {
                        string bearerToken = httpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                        string lastToken = redis.StringGet(uid.ToString());

                        if (bearerToken != lastToken)
                        {
                            TokenInvalidInfo tokenInvalidInfo = new()
                            {
                                StatusCode = 401
                            };

                            CreateResponse result = new(false, tokenInvalidInfo, "InvalidToken");
                            await httpContext.Response.WriteAsJsonAsync(result);
                            return;
                        }
                    }
                }
            }

            await _next(httpContext);
        }
    }
}
