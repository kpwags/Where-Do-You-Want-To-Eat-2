using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wheredoyouwanttoeat2.Models
{
    public class RestaurantTag
    {
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}