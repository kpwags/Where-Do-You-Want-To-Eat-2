using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace wheredoyouwanttoeat2.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        public virtual List<Restaurant> Restaurants { get; set; }
    }

    // public class UserRole : IdentityUserRole<int> { }
    // public class UserClaim : IdentityUserClaim<int> { }
    // public class UserLogin : IdentityUserLogin<int> { }
}