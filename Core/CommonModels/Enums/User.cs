using Microsoft.AspNetCore.Identity;

namespace Core.CommonModels.Enums
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenCreateTime { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
