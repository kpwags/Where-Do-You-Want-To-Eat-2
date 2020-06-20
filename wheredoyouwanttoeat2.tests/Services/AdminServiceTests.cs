using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Services;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Repositories;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Services
{
    public class AdminServiceTests
    {
        [Fact]
        public void Task_GetRestaurantById_ReturnsResult()
        {
            var singleRestaurant = new Restaurant
            {
                RestaurantId = 1,
                Name = "Test Restaurant",
                PhoneNumber = "215.666.5543",
                Website = "https://www.fakerestaurant.com",
                Latitude = 0,
                Longitude = 0
            };

            var restaurants = new List<Restaurant>();

            restaurants.Add(singleRestaurant);

            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockGet(restaurants.AsQueryable());
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            var result = adminService.GetRestaurantById(1);

            Assert.Equal(singleRestaurant, result);
        }

        [Fact]
        public void Task_GetRestaurantById_ReturnsNull()
        {
            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockGet(new List<Restaurant>().AsQueryable());
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            var result = adminService.GetRestaurantById(1);

            Assert.Null(result);
        }

        // TODO: Add Restaurant
        [Fact]
        public void Task_AddRestaurant_ValidRestaurant()
        {
            var restaurant = new Restaurant
            {
                Name = "Test Restaurant",
                PhoneNumber = "215.666.5543",
                Website = "https://www.fakerestaurant.com",
                Latitude = 0,
                Longitude = 0
            };

            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockAdd(restaurant);
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            var result = adminService.AddRestaurant(restaurant);


            Assert.Equal(restaurant, result.Result);
        }

        // TODO: Add Invalid Restaurant

        // TODO: Update Restaurant

        // TODO: Delete Restaurant
    }
}