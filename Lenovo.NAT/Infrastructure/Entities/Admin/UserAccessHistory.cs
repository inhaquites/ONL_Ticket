namespace Lenovo.NAT.Infrastructure.Entities.Admin
{
    public class UserAccessHistory
    {
        public UserAccessHistory(string userId, string userName, string page) 
        { 
            UserId = userId;
            UserName = userName;
            Page = page;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Page { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(-3);
    }
}