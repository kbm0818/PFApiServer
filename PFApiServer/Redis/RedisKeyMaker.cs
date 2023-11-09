namespace PFApiServer.Redis
{
    public static class RedisKeyMaker
    {
        public static string MakeIDKey(string id) => "UID_" + id;
        public static string MakeUserLockKey(string id) => "ULock_" + id;
    }
}
