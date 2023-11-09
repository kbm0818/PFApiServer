namespace PFApiServer.Redis
{
    public class RdbAuthUserData
    {
        public string Id { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public long Uid { get; set; } = 0;
        public string State { get; set; } = ""; // enum UserState    
    }
}
