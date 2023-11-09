using System.Data;
using Common.Enum;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace PFApiServer.RDB
{
    public class GlobalDB : IGlobalDB
    {
        private readonly IOptions<DBConfig> _dbConfig;
        private readonly ILogger<GlobalDB> _logger;
        private IDbConnection _dbConn;
        private readonly SqlKata.Compilers.MySqlCompiler _compiler;
        private readonly QueryFactory _queryFactory;

        public GlobalDB(ILogger<GlobalDB> logger, IOptions<DBConfig> dbConfig)
        {
            _dbConfig = dbConfig;
            _logger = logger;

            //Open();
            _dbConn = new MySqlConnection(_dbConfig.Value.GlobalDb);
            _dbConn.Open();

            _compiler = new SqlKata.Compilers.MySqlCompiler();
            _queryFactory = new QueryFactory(_dbConn, _compiler);
        }

        public void Dispose()
        {
            _dbConn.Close();
        }

        public async Task<ErrorCode> CreateAccountAsync(string id, string pw)
        {
            try
            {
                string saltValue = Security.SaltString();
                string hashingPassword = Security.MakeHashingPassWord(saltValue, pw);
                _logger.ZLogDebug(
                    $"[CreateAccount] Id: {id}, HashingPassword:{hashingPassword}");

                int count = await _queryFactory.Query("user").InsertAsync(new
                {
                    id = id,
                    hashingPassword = hashingPassword,
                    saltValue = saltValue
                });

                return count != 1 ? ErrorCode.CreateAccountFailInsert : ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[AccountDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, Id: {id}");
                return ErrorCode.CreateAccountFailException;
            }
        }

        public async Task<Tuple<ErrorCode, long>> VerifyUser(string id, string pw)
        {
            try
            {
                DAO.GlobalDB.User userInfo = await _queryFactory.Query("user")
                                        .Where("id", id)
                                        .FirstOrDefaultAsync< DAO.GlobalDB.User >();

                if (userInfo is null || userInfo.Uid == 0)
                {
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailUserNotExist, 0);
                }

                string hashingPassword = Security.MakeHashingPassWord(userInfo.SaltValue, pw);
                if (userInfo.HashingPassword != hashingPassword)
                {
                    _logger.ZLogError(
                        $"[GlobalDb.VerifyAccount] ErrorCode: {ErrorCode.LoginFailPwNotMatch}, Id: {id}");
                    return new Tuple<ErrorCode, long>(ErrorCode.LoginFailPwNotMatch, 0);
                }

                return new Tuple<ErrorCode, long>(ErrorCode.None, userInfo.Uid);
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[GlobalDb.VerifyAccount] ErrorCode: {ErrorCode.LoginFailException}, Id: {id}");
                return new Tuple<ErrorCode, long>(ErrorCode.LoginFailException, 0);
            }
        }

        //private void Open()
        //{
        //    _dbConn = new MySqlConnection(_dbConfig.Value.GlobalDb);

        //    _dbConn.Open();
        //}

        //private void Close()
        //{
        //    _dbConn.Close();
        //}
    }
}