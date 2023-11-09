namespace Common.Enum
{
    public enum ErrorCode : uint
    {
        None = 0,

        // Base
        UnhandleException = 1000,
        RedisFailException,
        InValidRequestHttpBody,
        AuthTokenFailWrongAuthToken,
        GetGameDbConnectionFail,

        // Account
        CreateAccountFailException = 2000,
        LoginFailException,
        LoginFailUserNotExist,
        LoginFailPwNotMatch,
        LoginFailSetAuthToken,
        AuthTokenMismatch,
        AuthTokenNotFound,
        AuthTokenFailWrongKeyword,
        AuthTokenFailSetNx,
        AccountIdMismatch,
        DuplicatedLogin,
        CreateAccountFailInsert,
        LoginFailAddRedis,
        CheckAuthFailNotExist,
        CheckAuthFailNotMatch,
        CheckAuthFailException,

    }
}
