using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wheredoyouwanttoeat2.Models
{
    public class RestaurantTag
    {
        public int RestaurantId { get; set; }

        public int TagId { get; set; }

        public Restaurant Restaurant { get; set; }

        public Tag Tag { get; set; }
    }
}