using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhereDoYouWantToEat2.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual List<RestaurantTag> RestaurantTags { get; set; }

        [NotMapped]
        public bool Selected { get; set; } = true;
    }
}