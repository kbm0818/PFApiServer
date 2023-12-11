using PFApiServer.Common;
using PFApiServer.Common.Enums;
using StackExchange.Redis;

namespace PFApiServer.Middleware
{
    /// <summary>
    /// 서버의 점검여부를 체크하기 위한 미들웨어
    /// 서버의 상태는 레디스에 저장되며
    /// 매번 호출되어 점검 여부를 체크한다
    /// </summary>
    public class MaintenanceMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConnectionMultiplexer _redis = redis;

        public async Task Invoke(HttpContext httpContext)
        {
            var ip = httpContext.Request.HttpContext.Connection.RemoteIpAddress;
            if (ip is null ||
                ip.ToString() != "121.134.21.189")
            {
                var maintenanceInfo = IsMaintenanceModeEnabled();

                if (maintenanceInfo.Item1 is true)
                {
                    CreateResponse result = new(false, maintenanceInfo.Item2, "Maintenance", []);
                    await httpContext.Response.WriteAsJsonAsync(result);
                    return;
                }
            }

            await _next(httpContext);
        }

        private (bool, MaintenanceInfo?) IsMaintenanceModeEnabled()
        {
            var redis = _redis.GetDatabase((int)EnumRedisDB.Server);
            if (redis is null)
            {
                return (true, null);
            }

            string stateRedisKey = "MaintenanceState";
            string? maintenencaState = redis.StringGet(stateRedisKey);

            if (maintenencaState is not null)
            {
                return (true, null);
            }

            if (maintenencaState == "On")
            {
                string startRedisKey = "MaintenanceStartTime";
                string endRedisKey = "MaintenanceEndTime";

                var maintenencaStart = redis.StringGet(startRedisKey);
                var maintenencaend = redis.StringGet(endRedisKey);

                return (true, new() { StartTime = maintenencaStart.ToString(), EndTime = maintenencaend.ToString() });
            }

            return (false, null);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMaintenanceMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceMiddleware>();
        }
    }

    public class MaintenanceInfo
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}
