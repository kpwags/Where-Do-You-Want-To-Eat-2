using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;

namespace wheredoyouwanttoeat2.tests.Mocks.Services
{
    public class MockHomeService : Mock<IHomeService>
    {
        public MockHomeService MockGetUserTags(IEnumerable<Tag> tags)
        {
            Setup(x => x.GetUserTags(It.IsAny<string>()))
                .Returns(tags);

            return this;
        }

        public MockHomeService MockGetUserRestaurants(IEnumerable<Restaurant> restaurants)
        {
            Setup(x => x.GetUserRestaurants(It.IsAny<string>()))
                .Returns(restaurants);

            return this;
        }

        public MockHomeService MockGetUserRestaurantsWithTags(IEnumerable<Restaurant> restaurants)
        {
            Setup(x => x.GetUserRestaurantsWithTags(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(restaurants);

            return this;
        }
    }
}