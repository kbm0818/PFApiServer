using Common.Enum;

namespace PFApiServer.RDB
{
    public interface IGlobalDB : IDisposable
    {
        public Task<ErrorCode> CreateAccountAsync(String id, String pw);

        public Task<Tuple<ErrorCode, Int64>> VerifyUser(String id, String pw);
    }
}
