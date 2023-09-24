namespace LibraryAPI.Models
{
    public class UserSession
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string AuthToken { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
