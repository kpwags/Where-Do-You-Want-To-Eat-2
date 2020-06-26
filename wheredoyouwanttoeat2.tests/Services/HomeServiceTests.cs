using Moq;
using System.Collections.Generic;
using System.Linq;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Services;
using wheredoyouwanttoeat2.tests.Mocks.Repositories;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;


namespace wheredoyouwanttoeat2.tests.Services
{
    public class HomeServiceTests
    {
        private User _loggedInUser;
        private List<RestaurantTag> _restaurantTags;
        private List<Restaurant> _restaurants;

        public HomeServiceTests()
        {
            _loggedInUser = new User
            {
                Id = "12345",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz"
            };


            _restaurantTags = new List<RestaurantTag>
            {
                new RestaurantTag
                {
                    RestaurantId = 1,
                    Restaurant = new Restaurant
                    {
                        UserId = "12345",
                        User = _loggedInUser,
                        Name = "Restaurant 1"

                    },
                    TagId = 1,
                    Tag = new Tag { TagId = 1, Name = "Mexican" }
                },
                new RestaurantTag
                {
                    RestaurantId = 1,
                    Restaurant = new Restaurant
                    {
                        UserId = "12345",
                        User = _loggedInUser,
                        Name = "Restaurant 1"

                    },
                    TagId = 2,
                    Tag = new Tag { TagId = 2, Name = "Tacos" }
                },
                new RestaurantTag
                {
                    RestaurantId = 2,
                    Restaurant = new Restaurant
                    {
                        UserId = "12345",
                        User = _loggedInUser,
                        Name = "Restaurant 2"

                    },
                    TagId = 3,
                    Tag = new Tag { TagId = 3, Name = "Italian" }
                },
                new RestaurantTag
                {
                    RestaurantId = 2,
                    Restaurant = new Restaurant
                    {
                        UserId = "12345",
                        User = _loggedInUser,
                        Name = "Restaurant 2"

                    },
                    TagId = 4,
                    Tag = new Tag { TagId = 4, Name = "Pasta" }
                }
            };

            _restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    UserId = "12345",
                    User = _loggedInUser,
                    Name = "Restaurant 1"
                },
                new Restaurant
                {
                    UserId = "12345",
                    User = _loggedInUser,
                    Name = "Restaurant 1"
                }
            };
        }

        [Fact]
        public void HomeService_GetUserTags_ReturnsList()
        {
            var mockRestaurantRespository = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRespository = new MockRepository<RestaurantTag>().MockGet(_restaurantTags.AsQueryable());
            var mockUserProvider = new MockUserProvider().MockGetUserId(_loggedInUser.Id);

            var homeService = new HomeService(mockRestaurantRespository, mockRestaurantTagRespository.Object, mockUserProvider.Object);

            var result = homeService.GetUserTags();

            Assert.IsType<List<Tag>>(result);
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void HomeService_GetUserRestaurants_ReturnsList()
        {
            var mockRestaurantRespository = new MockRepository<Restaurant>().MockGet(_restaurants.AsQueryable());
            var mockRestaurantTagRespository = Mock.Of<IRepository<RestaurantTag>>();
            var mockUserProvider = new MockUserProvider().MockGetUserId(_loggedInUser.Id);

            var homeService = new HomeService(mockRestaurantRespository.Object, mockRestaurantTagRespository, mockUserProvider.Object);

            var result = homeService.GetUserRestaurants();

            Assert.IsType<List<Restaurant>>(result.ToList());
            Assert.Equal(2, (int)result.Count());
        }

        [Fact]
        public void HomeService_GetUserRestaurantsWithTags_ReturnsList()
        {
            var tags = new List<int> { 1, 3 };

            var mockRestaurantRespository = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRespository = new MockRepository<RestaurantTag>().MockGet(_restaurantTags.AsQueryable().Where(rt => tags.Contains(rt.TagId) && rt.Restaurant.UserId == _loggedInUser.Id));
            var mockUserProvider = new MockUserProvider().MockGetUserId(_loggedInUser.Id);

            var homeService = new HomeService(mockRestaurantRespository, mockRestaurantTagRespository.Object, mockUserProvider.Object);

            var result = homeService.GetUserRestaurantsWithTags(tags);

            Assert.IsType<List<Restaurant>>(result.ToList());
            Assert.Equal(2, (int)result.Count());
        }

        [Fact]
        public void HomeService_GetUserRestaurantsWithTags_ReturnsEmpty()
        {
            var tags = new List<int>();

            var mockRestaurantRespository = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRespository = new MockRepository<RestaurantTag>().MockGet(_restaurantTags.AsQueryable().Where(rt => tags.Contains(rt.TagId) && rt.Restaurant.UserId == _loggedInUser.Id));
            var mockUserProvider = new MockUserProvider().MockGetUserId(_loggedInUser.Id);

            var homeService = new HomeService(mockRestaurantRespository, mockRestaurantTagRespository.Object, mockUserProvider.Object);

            var result = homeService.GetUserRestaurantsWithTags(tags);

            Assert.IsType<List<Restaurant>>(result.ToList());
            Assert.Equal(0, (int)result.Count());
        }
    }
}