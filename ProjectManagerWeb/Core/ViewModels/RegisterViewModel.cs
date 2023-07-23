using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "User name")]
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "User role")]
        [Required(ErrorMessage = "User role is required")]
        public UserRoles UserRole { get; set; }
        //public string ReturnUrl { get; set; }
    }
}
