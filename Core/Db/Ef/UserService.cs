using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Db.Ef
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }
        public string GetMyId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return result;
        }
        public string GetCookiesRefreshToken()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = Convert.ToString(_httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Identity.Application"]);
            }
            return result;
        }
    }
}
