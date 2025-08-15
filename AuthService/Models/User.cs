// Models/User.cs
namespace AuthService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Profile { get; set; } = string.Empty;
        public int? MobileNo { get; set; } 
        public string Role { get; set; } = "user";
    }
}