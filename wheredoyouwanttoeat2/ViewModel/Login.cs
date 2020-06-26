using System.ComponentModel.DataAnnotations;


namespace WhereDoYouWantToEat2.ViewModel
{
    public class Login : BaseViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}