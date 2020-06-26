using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Controllers;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Controllers
{
    public class AdminControllerTests
    {
        private ITempDataDictionary _tempData;

        public AdminControllerTests()
        {
            ITempDataProvider tempDataProvider = Mock.Of<ITempDataProvider>();
            TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            _tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
        }

        [Fact]
        public async Task AdminController_ListRestaurants()
        {
            var mockRestaurantsList = new List<Restaurant>
            {
                new Restaurant {
                    RestaurantId = 1,
                    Name = "Restaurant One",
                    RestaurantTags = new List<RestaurantTag>
                    {
                        new RestaurantTag
                        {
                            RestaurantId = 1,
                            Tag = new Tag
                            {
                                Name = "Mexican"
                            }
                        },
                        new RestaurantTag
                        {
                            RestaurantId = 1,
                            Tag = new Tag
                            {
                                Name = "Tacos"
                            }
                        }
                    }
                },
                new Restaurant {
                    RestaurantId = 2,
                    Name = "Restaurant Two",
                    RestaurantTags = new List<RestaurantTag>
                    {
                        new RestaurantTag
                        {
                            RestaurantId = 2,
                            Tag = new Tag
                            {
                                Name = "Italian"
                            }
                        },
                        new RestaurantTag
                        {
                            RestaurantId = 2,
                            Tag = new Tag
                            {
                                Name = "Pasta"
                            }
                        }
                    }
                },
            };

            var mockAdminService = new MockAdminService().MockGetUserRestaurants(mockRestaurantsList);
            var mockLogger = Mock.Of<ILogger<AdminController>>();
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(new User { Id = "12345", Name = "Test User", Email = "test@wheredoyouwanttoeat.xyz" });

            var controller = new AdminController(mockUserProvider.Object, mockLogger, mockAdminService.Object);
            controller.TempData = _tempData;

            var result = await controller.Restaurants();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.RestaurantAdmin>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.RestaurantAdmin>(model);
            Assert.Equal(2, model.Restaurants.Count);
        }

        [Fact]
        public async Task AdminController_AddRestaurant()
        {
            var loggedInUser = new User
            {
                Id = "1234567890",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var restaurant = new Restaurant
            {
                Name = "Test Restaurant",
                AddressLine1 = "",
                AddressLine2 = "",
                City = "",
                State = "",
                ZipCode = "",
                Website = "https://www.testrestaurant.com",
                Menu = "https://www.testrestaurant.com/menu",
                PhoneNumber = "(215) 555 - 5555",
                Latitude = 0,
                Longitude = 0,
                User = loggedInUser,
                UserId = loggedInUser.Id,
                TagString = "Burgers,Fries,Beer"
            };

            var mockAdminService = new MockAdminService().MockAddRestaurant(restaurant);
            var loggerMoq = Mock.Of<ILogger<AdminController>>();
            var userProviderMoq = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            var controller = new AdminController(userProviderMoq.Object, loggerMoq, mockAdminService.Object);

            var result = (RedirectToActionResult)await controller.AddRestaurant(restaurant);

            Assert.Equal("Restaurants", result.ActionName);
        }

        [Fact]
        public async Task AdminController_AddInvalidRestaurant()
        {
            var loggedInUser = new User
            {
                Id = "1234567890",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var restaurant = new Restaurant
            {
                Website = "https://www.testrestaurant.com",
                Menu = "https://www.testrestaurant.com/menu",
                PhoneNumber = "(215) 555 - 5555",
                TagString = "Burgers,Fries,Beer"
            };

            await Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(async () =>
            {
                var mockAdminService = new MockAdminService().MockAddRestaurant(restaurant);
                var loggerMoq = Mock.Of<ILogger<AdminController>>();
                var userProviderMoq = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

                var controller = new AdminController(userProviderMoq.Object, loggerMoq, mockAdminService.Object);

                await controller.AddRestaurant(restaurant);
            });
        }

        [Fact]
        public async Task AdminController_UpdateRestaurant()
        {
            var loggedInUser = new User
            {
                Id = "1234567890",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var restaurant = new Restaurant
            {
                Name = "Test Restaurant",
                AddressLine1 = "",
                AddressLine2 = "",
                City = "",
                State = "",
                ZipCode = "",
                Website = "https://www.testrestaurant.com",
                Menu = "https://www.testrestaurant.com/menu",
                PhoneNumber = "(215) 555 - 5555",
                Latitude = 0,
                Longitude = 0,
                User = loggedInUser,
                UserId = loggedInUser.Id,
                TagString = "Burgers,Fries,Beer"
            };

            var mockAdminService = new MockAdminService().MockUpdateRestaurant(restaurant);
            mockAdminService.MockGetRestaurantById(restaurant);
            var loggerMoq = Mock.Of<ILogger<AdminController>>();
            var userProviderMoq = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            var controller = new AdminController(userProviderMoq.Object, loggerMoq, mockAdminService.Object);

            var result = (RedirectToActionResult)await controller.EditRestaurant(restaurant);

            Assert.Equal("Restaurants", result.ActionName);
        }

        [Fact]
        public async Task AdminController_DeleteRestaurant()
        {
            var loggedInUser = new User
            {
                Id = "1234567890",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var restaurant = new Restaurant
            {
                Name = "Test Restaurant",
                AddressLine1 = "",
                AddressLine2 = "",
                City = "",
                State = "",
                ZipCode = "",
                Website = "https://www.testrestaurant.com",
                Menu = "https://www.testrestaurant.com/menu",
                PhoneNumber = "(215) 555 - 5555",
                Latitude = 0,
                Longitude = 0,
                User = loggedInUser,
                UserId = loggedInUser.Id,
                TagString = "Burgers,Fries,Beer"
            };

            var mockAdminService = new MockAdminService().MockDeleteRestaurant(restaurant);
            mockAdminService.MockGetRestaurantById(restaurant);

            var loggerMoq = Mock.Of<ILogger<AdminController>>();
            var userProviderMoq = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            var controller = new AdminController(userProviderMoq.Object, loggerMoq, mockAdminService.Object);

            var result = (RedirectToActionResult)await controller.DeleteRestaurant(restaurant);

            Assert.Equal("Restaurants", result.ActionName);
        }
    }
}