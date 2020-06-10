using System.Collections.Generic;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.ViewModel
{
    public class RestaurantAdmin : BaseViewModel
    {
        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}