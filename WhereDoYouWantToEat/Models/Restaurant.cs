using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WhereDoYouWantToEat2.Models
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

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Website")]
        public string Website { get; set; }

        [Display(Name = "Menu")]
        public string Menu { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal Longitude { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual List<RestaurantTag> RestaurantTags { get; set; }

        [NotMapped]
        public string TagString { get; set; }

        [NotMapped]
        public bool HasInformation
        {
            get
            {
                if ((AddressLine1 != null && AddressLine1 != "") || (PhoneNumber != null && PhoneNumber != "") || (Website != null && Website != "") || (Menu != null && Menu != ""))
                {
                    return true;
                }

                return false;
            }
        }

        [NotMapped]
        public bool HasFullAddress
        {
            get
            {
                if ((AddressLine1 != null && AddressLine1 != "") || (City != null && City != "") || (State != null && State != "") || (ZipCode != null && ZipCode != ""))
                {
                    return true;
                }

                return false;
            }
        }

        [NotMapped]
        public string FullAddress
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"{AddressLine1} ");

                if (AddressLine2 != "" && AddressLine2 != null)
                {
                    builder.Append($"{AddressLine1} ");
                }

                builder.Append($"{City}, {State} {ZipCode}");

                return builder.ToString();
            }
        }

        public static bool HasAddressChanged(Restaurant restaurant1, Restaurant restaurant2)
        {
            if (restaurant1.AddressLine1 == restaurant2.AddressLine1 &&
                    restaurant1.AddressLine2 == restaurant2.AddressLine2 &&
                    restaurant1.City == restaurant2.City &&
                    restaurant1.State == restaurant2.State &&
                    restaurant1.ZipCode == restaurant2.ZipCode)
            {
                return false;
            }

            return true;
        }

        public bool IsValid()
        {
            if (Name == null || Name.Trim() == "")
            {
                return false;
            }

            return true;
        }
    }
}