using System.Collections.Generic;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.ViewModel
{
    public class ChooserViewModel
    {
        public Restaurant SelectedRestaurant { get; set; }
        public int ChoiceCount { get; set; }
        public string ButtonText { get; set; }
        public List<Tag> Tags { get; set; }

        public string LeadingText { get; set; }
    }
}