using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Classes;
using wheredoyouwanttoeat2.Controllers;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Controllers
{
    public class HomeControllerTests
    {
        private User _loggedInUser;
        private ITempDataDictionary _tempData;

        public HomeControllerTests()
        {
            _loggedInUser = new User
            {
                Id = "12345",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz"
            };

            ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
            TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            _tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
        }

        [Fact]
        public async Task HomeController_Index_ListTagsForUser()
        {
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(_loggedInUser);
            var mockLogger = Mock.Of<ILogger<HomeController>>();
            var mockHomeService = new MockHomeService();

            var tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1" },
                new Tag { TagId = 2, Name = "tag2" },
                new Tag { TagId = 3, Name = "tag3" }
            };
            mockHomeService.MockGetUserTags(tags);

            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" },
                new Restaurant { RestaurantId = 3, Name = "Restaurant 3" },
                new Restaurant { RestaurantId = 4, Name = "Restaurant 4" },
                new Restaurant { RestaurantId = 5, Name = "Restaurant 5" }
            };
            mockHomeService.MockGetUserRestaurants(restaurants);

            var homeController = new HomeController(mockUserProvider.Object, mockLogger, mockHomeService.Object);
            homeController.TempData = _tempData;

            var result = await homeController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Randomizer>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Randomizer>(model);
            Assert.Equal(3, model.Tags.Count);
            Assert.Equal(5, model.RestaurantCount);
        }

        [Fact]
        public async Task HomeController_Index_GetsRestaurantWithTags()
        {
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(_loggedInUser);
            var mockLogger = Mock.Of<ILogger<HomeController>>();
            var mockHomeService = new MockHomeService();

            var tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1" },
                new Tag { TagId = 2, Name = "tag2" },
                new Tag { TagId = 3, Name = "tag3" }
            };
            mockHomeService.MockGetUserTags(tags);

            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" },
                new Restaurant { RestaurantId = 3, Name = "Restaurant 3" },
                new Restaurant { RestaurantId = 4, Name = "Restaurant 4" },
                new Restaurant { RestaurantId = 5, Name = "Restaurant 5" }
            };
            mockHomeService.MockGetUserRestaurantsWithTags(restaurants);

            _tempData["choice_count"] = 0;

            var homeController = new HomeController(mockUserProvider.Object, mockLogger, mockHomeService.Object);
            homeController.TempData = _tempData;

            var randomizerViewModel = new ViewModel.Randomizer
            {
                Tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1", Selected = true },
                new Tag { TagId = 2, Name = "tag2", Selected = true },
                new Tag { TagId = 3, Name = "tag3", Selected = false }
            }
            };

            var result = await homeController.Index(randomizerViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Randomizer>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Randomizer>(model);
            Assert.Contains(model.SelectedRestaurant, restaurants);
        }

        [Fact]
        public async Task HomeController_Index_GetsRestaurantWithNoTags()
        {
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(_loggedInUser);
            var mockLogger = Mock.Of<ILogger<HomeController>>();
            var mockHomeService = new MockHomeService();

            var tags = new List<Tag>();
            mockHomeService.MockGetUserTags(tags);

            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" },
                new Restaurant { RestaurantId = 3, Name = "Restaurant 3" },
                new Restaurant { RestaurantId = 4, Name = "Restaurant 4" },
                new Restaurant { RestaurantId = 5, Name = "Restaurant 5" }
            };
            mockHomeService.MockGetUserRestaurants(restaurants);

            _tempData["choice_count"] = 0;

            var homeController = new HomeController(mockUserProvider.Object, mockLogger, mockHomeService.Object);
            homeController.TempData = _tempData;

            var randomizerViewModel = new ViewModel.Randomizer
            {
                Tags = new List<Tag>()
            };

            var result = await homeController.Index(randomizerViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Randomizer>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Randomizer>(model);
            Assert.Contains(model.SelectedRestaurant, restaurants);
        }

        [Fact]
        public async Task HomeController_Index_NoTagsSelected()
        {
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(_loggedInUser);
            var mockLogger = Mock.Of<ILogger<HomeController>>();
            var mockHomeService = new MockHomeService();

            var tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1" },
                new Tag { TagId = 2, Name = "tag2" },
                new Tag { TagId = 3, Name = "tag3" }
            };
            mockHomeService.MockGetUserTags(tags);

            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" },
                new Restaurant { RestaurantId = 3, Name = "Restaurant 3" },
                new Restaurant { RestaurantId = 4, Name = "Restaurant 4" },
                new Restaurant { RestaurantId = 5, Name = "Restaurant 5" }
            };
            mockHomeService.MockGetUserRestaurantsWithTags(restaurants);

            _tempData["choice_count"] = 0;

            var homeController = new HomeController(mockUserProvider.Object, mockLogger, mockHomeService.Object);
            homeController.TempData = _tempData;

            var randomizerViewModel = new ViewModel.Randomizer
            {
                Tags = new List<Tag>
                {
                    new Tag { TagId = 1, Name = "tag1", Selected = false },
                    new Tag { TagId = 2, Name = "tag2", Selected = false },
                    new Tag { TagId = 3, Name = "tag3", Selected = false }
                }
            };

            var result = await homeController.Index(randomizerViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Randomizer>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Randomizer>(model);
            Assert.Equal("Please select at least one tag", model.ErrorMessage);
        }

        [Fact]
        public async Task HomeController_Index_GetsRestaurantWithTags_TooManyChoices()
        {
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(_loggedInUser);
            var mockLogger = Mock.Of<ILogger<HomeController>>();
            var mockHomeService = new MockHomeService();

            var tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1" },
                new Tag { TagId = 2, Name = "tag2" },
                new Tag { TagId = 3, Name = "tag3" }
            };
            mockHomeService.MockGetUserTags(tags);

            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" },
                new Restaurant { RestaurantId = 3, Name = "Restaurant 3" },
                new Restaurant { RestaurantId = 4, Name = "Restaurant 4" },
                new Restaurant { RestaurantId = 5, Name = "Restaurant 5" }
            };
            mockHomeService.MockGetUserRestaurantsWithTags(restaurants);

            _tempData["choice_count"] = 5;

            var homeController = new HomeController(mockUserProvider.Object, mockLogger, mockHomeService.Object);
            homeController.TempData = _tempData;

            var randomizerViewModel = new ViewModel.Randomizer
            {
                Tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1", Selected = true },
                new Tag { TagId = 2, Name = "tag2", Selected = true },
                new Tag { TagId = 3, Name = "tag3", Selected = false }
            }
            };

            var result = await homeController.Index(randomizerViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Randomizer>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Randomizer>(model);
            Assert.Contains(model.SelectedRestaurant, restaurants);
            Assert.Contains(model.LeadingText, Utilities.TooManyChoices);
        }

        [Fact]
        public async Task HomeController_Index_GetsRestaurantWithTags_WayTooManyChoices()
        {
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(_loggedInUser);
            var mockLogger = Mock.Of<ILogger<HomeController>>();
            var mockHomeService = new MockHomeService();

            var tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1" },
                new Tag { TagId = 2, Name = "tag2" },
                new Tag { TagId = 3, Name = "tag3" }
            };
            mockHomeService.MockGetUserTags(tags);

            var restaurants = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant 1" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant 2" },
                new Restaurant { RestaurantId = 3, Name = "Restaurant 3" },
                new Restaurant { RestaurantId = 4, Name = "Restaurant 4" },
                new Restaurant { RestaurantId = 5, Name = "Restaurant 5" }
            };
            mockHomeService.MockGetUserRestaurantsWithTags(restaurants);

            _tempData["choice_count"] = 12;

            var homeController = new HomeController(mockUserProvider.Object, mockLogger, mockHomeService.Object);
            homeController.TempData = _tempData;

            var randomizerViewModel = new ViewModel.Randomizer
            {
                Tags = new List<Tag>
            {
                new Tag { TagId = 1, Name = "tag1", Selected = true },
                new Tag { TagId = 2, Name = "tag2", Selected = true },
                new Tag { TagId = 3, Name = "tag3", Selected = false }
            }
            };

            var result = await homeController.Index(randomizerViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Randomizer>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Randomizer>(model);
            Assert.Contains(model.SelectedRestaurant, restaurants);
            Assert.Contains(model.LeadingText, Utilities.WayTooManyChoices);
        }
    }
}