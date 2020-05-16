using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wheredoyouwanttoeat2.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Display(Name = "Address Line 1")]
        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [StringLength(255)]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        [StringLength(2)]
        public string State { get; set; }

        [StringLength(10)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public List<RestaurantTag> RestaurantTags { get; set; }

        [NotMapped]
        public string Tags { get; set; }
    }
}