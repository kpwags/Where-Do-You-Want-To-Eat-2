using System.Linq;
using System.Collections.Generic;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.Services
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<RestaurantTag> _restaurantTagRepository;

        public HomeService(IRepository<Restaurant> restaurantRepository, IRepository<RestaurantTag> restaurantTagRepository)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantTagRepository = restaurantTagRepository;
        }

        public IEnumerable<Tag> GetUserTags(string userId)
        {
            var tags = _restaurantTagRepository.Get(rt => rt.Restaurant.UserId == userId).OrderBy(rt => rt.Tag.Name).Select(rt => rt.Tag);

            return tags.AsEnumerable().GroupBy(t => t.TagId).Select(t => t.First()).ToList();
        }

        public IEnumerable<Restaurant> GetUserRestaurants(string userId)
        {
            return _restaurantRepository.Get(r => r.UserId == userId);
        }

        public IEnumerable<Restaurant> GetUserRestaurantsWithTags(string userId, List<int> tags)
        {
            return _restaurantTagRepository.Get(rt => tags.Contains(rt.TagId)).Select(rt => rt.Restaurant);
        }
    }
}