using System.ComponentModel.DataAnnotations;

namespace WhereDoYouWantToEat2.ViewModel
{
    public class EditProfile : BaseViewModel
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
    }
}