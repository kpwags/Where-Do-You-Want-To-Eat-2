using System.ComponentModel.DataAnnotations;

namespace wheredoyouwanttoeat2.ViewModel
{
    public class DeleteAccount
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}