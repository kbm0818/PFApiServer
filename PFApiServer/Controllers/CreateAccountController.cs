using Common.DTO;
using Common.Enum;
using Microsoft.AspNetCore.Mvc;
using PFApiServer.RDB;
using static PFApiServer.LogManager;
using ZLogger;

namespace PFApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccount : ControllerBase
    {
        private readonly IGlobalDB _globalDb;
        private readonly ILogger<CreateAccount> _logger;

        public CreateAccount(ILogger<CreateAccount> logger, IGlobalDB accountDb)
        {
            _logger = logger;
            _globalDb = accountDb;
        }

        [HttpPost]
        public async Task<CreateAccountRes> Post(CreateAccountReq request)
        {
            var response = new CreateAccountRes();

            var errorCode = await _globalDb.CreateAccountAsync(request.Id, request.Password);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                return response;
            }

            _logger.ZLogInformationWithPayload(EventIdDic[EventType.CreateAccount], new { Id = request.Id }, $"CreateAccount Success");
            return response;
        }
    }
}
