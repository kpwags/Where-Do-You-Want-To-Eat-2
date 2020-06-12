using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using wheredoyouwanttoeat2.Controllers;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void RestaurantsTest_ReturnsViewWithRestaurants()
        {
            var mockRestaurantsList = new List<Restaurant>
            {
                new Restaurant { RestaurantId = 1, Name = "Restaurant One", TagString = "Italian, Wine, Happy Hour" },
                new Restaurant { RestaurantId = 2, Name = "Restaurant Two", TagString = "Mexican, Margaritas, Spicy" }
            };

            var mockAdminService = new MockAdminService().MockGetUserRestaurants(mockRestaurantsList);
            var loggerMoq = Mock.Of<ILogger<AdminController>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var controller = new AdminController(userProviderMoq, loggerMoq, mockAdminService.Object);

            var result = controller.Restaurants();

            Assert.IsAssignableFrom<ViewResult>(result);
        }
    }
}