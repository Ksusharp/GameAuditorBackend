using Users.App.Models;

namespace GameAuditor.Models.ViewModels
{
    public class LoginResponse
    {
        public Token Token { get; set; }

        public string RefreshToken { get; set; } 
    }
}
