using System.Linq;
using System.Collections.Generic;
using WhereDoYouWantToEat2.Services.Interfaces;
using WhereDoYouWantToEat2.Respositories.Interfaces;
using WhereDoYouWantToEat2.Models;

namespace WhereDoYouWantToEat2.Services
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<RestaurantTag> _restaurantTagRepository;
        private readonly IUserProvider _userProvider;

        public HomeService(IRepository<Restaurant> restaurantRepository, IRepository<RestaurantTag> restaurantTagRepository, IUserProvider provider)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantTagRepository = restaurantTagRepository;
            _userProvider = provider;
        }

        public IEnumerable<Tag> GetUserTags(string userId = "")
        {
            if (userId == "")
            {
                userId = _userProvider.GetUserId();
            }

            var tags = _restaurantTagRepository.Get(rt => rt.Restaurant.UserId == userId).OrderBy(rt => rt.Tag.Name).Select(rt => rt.Tag);

            return tags.AsEnumerable().GroupBy(t => t.TagId).Select(t => t.First()).ToList();
        }

        public IEnumerable<Restaurant> GetUserRestaurants(string userId = "")
        {
            if (userId == "")
            {
                userId = _userProvider.GetUserId();
            }

            return _restaurantRepository.Get(r => r.UserId == userId);
        }

        public IEnumerable<Restaurant> GetUserRestaurantsWithTags(List<int> tags, string userId = "")
        {
            if (userId == "")
            {
                userId = _userProvider.GetUserId();
            }

            return _restaurantTagRepository.Get(rt => tags.Contains(rt.TagId) && rt.Restaurant.UserId == userId).Select(rt => rt.Restaurant);
        }
    }
}