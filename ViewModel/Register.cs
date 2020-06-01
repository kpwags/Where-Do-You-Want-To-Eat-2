using System.ComponentModel.DataAnnotations;

namespace wheredoyouwanttoeat2.ViewModel
{
    public class Register : BaseViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Your password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }
}