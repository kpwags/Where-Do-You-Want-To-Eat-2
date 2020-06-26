using System.Collections.Generic;
using WhereDoYouWantToEat2.Models;

namespace WhereDoYouWantToEat2.Services.Interfaces
{
    public interface IHomeService
    {
        IEnumerable<Tag> GetUserTags(string userId = "");

        IEnumerable<Restaurant> GetUserRestaurants(string userId = "");

        IEnumerable<Restaurant> GetUserRestaurantsWithTags(List<int> tags, string userId = "");
    }
}