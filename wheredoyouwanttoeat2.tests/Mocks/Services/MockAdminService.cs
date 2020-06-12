using Moq;
using System.Collections.Generic;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;

namespace wheredoyouwanttoeat2.tests.Mocks.Services
{
    public class MockAdminService : Mock<IAdminService>
    {
        public MockAdminService MockGetUserRestaurants(IEnumerable<Restaurant> restaurants)
        {
            Setup(x => x.GetUserRestaurants(It.IsAny<string>()))
                .Returns(restaurants);

            return this;
        }

        public MockAdminService MockGetRestaurantById(Restaurant restaurant)
        {
            Setup(x => x.GetRestaurantById(It.IsAny<int>()))
                .Returns(restaurant);

            return this;
        }

        public MockAdminService MockGetTagByName(Tag tag)
        {
            Setup(x => x.GetTagByName(It.IsAny<string>()))
                .Returns(tag);

            return this;
        }

        public MockAdminService MockGetRestaurantTags(IEnumerable<RestaurantTag> restaurantTags)
        {
            Setup(x => x.GetRestaurantTags(It.IsAny<int>()))
                .Returns(restaurantTags);

            return this;
        }
    }
}