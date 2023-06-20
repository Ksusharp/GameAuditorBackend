using System.ComponentModel.DataAnnotations;

namespace GameAuditor.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Password is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
