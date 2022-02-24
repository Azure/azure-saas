using System.ComponentModel.DataAnnotations;

namespace Saas.LandingSignup.Web.Models
{
    public class OnboardingFlow
    {
        [Required]
        [EmailAddress]
        [Display(Name = SR.EmailPrompt)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = SR.PasswordErrorMessageTemplate, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = SR.PasswordPrompt)]
        public string Password { get; set; }

        public string TenantId { get; set; }

        public string TenantUserName { get; set; }
    }
}