using System.Collections.Generic;
using WhereDoYouWantToEat2.Models;

namespace WhereDoYouWantToEat2.ViewModel
{
    public class Randomizer : BaseViewModel
    {
        public int RestaurantCount { get; set; }
        public Restaurant SelectedRestaurant { get; set; }
        public int ChoiceCount { get; set; }
        public string ButtonText { get; set; }
        public List<Tag> Tags { get; set; }
        public string LeadingText { get; set; }
    }
}