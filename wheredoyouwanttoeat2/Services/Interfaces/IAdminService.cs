using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.Services.Interfaces
{
    public interface IAdminService
    {
        IEnumerable<Restaurant> GetUserRestaurants(string userId = "");

        Restaurant GetRestaurantById(int id);

        Task<Restaurant> AddRestaurant(Restaurant restaurant);

        Task AddTagsToRestaurant(Restaurant restaurant, List<string> tags);

        Task UpdateRestaurantTags(Restaurant restaurant, List<string> tags);

        Task CleanUpTags(Restaurant restaurant, List<string> tags);

        Task<Restaurant> UpdateRestaurant(Restaurant restaurant, bool updateCoordinates = false);

        Task DeleteRestaurant(Restaurant restaurant);

        Tag GetTagByName(string tagName);

        IEnumerable<RestaurantTag> GetRestaurantTags(int restaurantId);
    }
}