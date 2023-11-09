namespace PFApiServer.DAO.GlobalDB
{
    public class User
    {
        public long Uid { get; set; }
        public string Id { get; set; } = string.Empty;
        public string HashingPassword { get; set; } = string.Empty;
        public string SaltValue { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
