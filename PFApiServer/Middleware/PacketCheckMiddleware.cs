using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PFApiServer.Common.Enums;
using PFApiServer.Common;
using StackExchange.Redis;

namespace PFApiServer.Middleware
{
    public class PacketCheckMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConnectionMultiplexer _redis = redis;

        public async Task Invoke(HttpContext httpContext)
        {
            var redis = _redis.GetDatabase((int)EnumRedisDB.Packet);

            httpContext.Request.Headers.TryGetValue("uid", out StringValues uid);
            httpContext.Request.Headers.TryGetValue("reqCount", out StringValues reqCount);

            List<string> excludeList = new()
            {
                "/Game/GameUser/GetGuestToken",
                "/Game/GameUser/AgreementInfo",
                "/Game/GameUser/AgreementSet",
                "/Game/GameUser/Login",
                "/Game/GameUser/CheckVersion"
            };

            if (httpContext.Request.Path.Value != null)
            {
                if (!excludeList.Contains(httpContext.Request.Path.Value))
                {
                    if (!uid.IsNullOrEmpty() && !reqCount.IsNullOrEmpty())
                    {

                        int currentReqcount = int.Parse(Convert.ToString(reqCount));
                        int lastReqCount = int.Parse(redis.StringGet(Convert.ToString(uid)));

                        if (lastReqCount > 1 && currentReqcount < lastReqCount)
                        {
                            var res = redis.StringGet(Convert.ToString(uid) + "_Packet").ToString();
                            if (!res.IsNullOrEmpty())
                            {
                                // 마지막 패킷 레디스에 값이 있으면 보낸다.
                                var okResult = new OkObjectResult(JsonConvert.DeserializeObject<CreateResponse>(res));
                                await httpContext.Response.WriteAsJsonAsync(okResult);
                                return;
                            }
                            else
                            {

                                PacketDuplicatedInfo packetDuplacatedInfo = new()
                                {
                                    PacketNumber = lastReqCount
                                };

                                CreateResponse result = new(false, packetDuplacatedInfo, "PacketDuplicated");
                                await httpContext.Response.WriteAsJsonAsync(result);
                                return;
                            }
                        }

                        //else if (currentReqcount > lastReqCount + 15)
                        //{
                        //    PacketDuplicatedInfo packetDuplacatedInfo = new()
                        //    {
                        //        PacketNumber = 0
                        //    };

                        //    CreateResponse result = new(false, packetDuplacatedInfo, "PacketDuplicated");
                        //    await httpContext.Response.WriteAsJsonAsync(result);
                        //    return;
                        //}

                        if (currentReqcount > lastReqCount + 15)
                        {
                            PacketDuplicatedInfo packetDuplacatedInfo = new()
                            {
                                PacketNumber = 0
                            };

                            CreateResponse result = new(false, packetDuplacatedInfo, "PacketDuplicated");
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
