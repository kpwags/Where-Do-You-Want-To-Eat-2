using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WhereDoYouWantToEat2.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public virtual List<Restaurant> Restaurants { get; set; }
    }
}