using System.Collections.Generic;
using WhereDoYouWantToEat2.Models;

namespace WhereDoYouWantToEat2.ViewModel
{
    public class RestaurantAdmin : BaseViewModel
    {
        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}