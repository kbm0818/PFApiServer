using Common.DTO;
using Common.Enum;
using Microsoft.AspNetCore.Mvc;
using PFApiServer.RDB;
using PFApiServer.Redis;
using static PFApiServer.LogManager;
using ZLogger;

namespace PFApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Login : ControllerBase
    {
        private readonly IGlobalDB _globalDb;
        private readonly IUserDB _userDb;
        private readonly ILogger<Login> _logger;

        public Login(ILogger<Login> logger, IGlobalDB accountDb, IUserDB memoryDb)
        {
            _logger = logger;
            _globalDb = accountDb;
            _userDb = memoryDb;
        }

        [HttpPost]
        public async Task<LoginResponse> Post(LoginRequest request)
        {
            LoginResponse response = new();

            // ID, PW 검증
            (ErrorCode errorCode, long accountId) = await _globalDb.VerifyUser(request.Id, request.Password);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                return response;
            }


            string authToken = Security.CreateAuthToken();
            errorCode = await _userDb.RegistUserAsync(request.Id, authToken, accountId);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                return response;
            }

            _logger.ZLogInformationWithPayload(EventIdDic[EventType.Login], new { request.Id, AuthToken = authToken, AccountId = accountId }, "Login Success");

            response.AuthToken = authToken;
            return response;
        }
    }
}
