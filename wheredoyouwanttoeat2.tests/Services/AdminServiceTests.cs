using Moq;
using System.Collections.Generic;
using System.Linq;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Services;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Repositories;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Services
{
    public class AdminServiceTests
    {
        public List<Restaurant> restaurants = new List<Restaurant>
        {
            new Restaurant { RestaurantId = 1, Name = "Restaurant 1", TagString = "Italian, Wine, Happy Hour" },
            new Restaurant { RestaurantId = 2, Name = "Restaurant 2", TagString = "Mexican, Margaritas, Spicy" },
            new Restaurant { RestaurantId = 3, Name = "Restaurant 3", TagString = "Wings, BBQ, Beer" },
            new Restaurant { RestaurantId = 4, Name = "Restaurant 4", TagString = "Sushi, Saki" },
            new Restaurant { RestaurantId = 5, Name = "Restaurant 5", TagString = "Burgers" }
        };

        [Fact]
        public void AdminService_GetRestaurantById_ReturnsResult()
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

            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockGetById(singleRestaurant);
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            var result = adminService.GetRestaurantById(1);

            Assert.Equal(singleRestaurant, result);
        }

        [Fact]
        public void AdminService_GetRestaurantById_ReturnsNull()
        {
            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockGet(new List<Restaurant>().AsQueryable());
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            var result = adminService.GetRestaurantById(1);

            Assert.Null(result);
        }

        [Fact]
        public void AdminService_AddRestaurant_ValidRestaurant()
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

        [Fact]
        public void AdminService_AddRestaurant_InvalidRestaurant()
        {
            var restaurant = new Restaurant
            {
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

            Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(async () =>
            {
                await adminService.AddRestaurant(restaurant);
            });
        }

        [Fact]
        public void AdminService_UpdateRestaurant_ValidRestaurant()
        {
            var restaurant = new Restaurant
            {
                RestaurantId = 1,
                Name = "Test Restaurant",
                PhoneNumber = "215.666.5543",
                Website = "https://www.fakerestaurant.com",
                Latitude = 10,
                Longitude = 10
            };

            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockUpdate(restaurant);
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            var result = adminService.UpdateRestaurant(restaurant);

            Assert.Equal(restaurant, result.Result);
        }

        [Fact]
        public void AdminService_UpdateRestaurant_InvalidRestaurant()
        {
            var restaurant = new Restaurant
            {
                RestaurantId = 1,
                PhoneNumber = "215.666.5543",
                Website = "https://www.fakerestaurant.com",
                Latitude = 0,
                Longitude = 0
            };

            var mockRestaurantRepoMoq = new MockRepository<Restaurant>().MockUpdate(restaurant);
            var mockRestaurantTagRepoMoq = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepoMoq = Mock.Of<IRepository<Tag>>();
            var userProviderMoq = Mock.Of<IUserProvider>();

            var adminService = new AdminService(mockRestaurantRepoMoq.Object, mockRestaurantTagRepoMoq, mockTagRepoMoq, userProviderMoq);

            Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(async () =>
            {
                await adminService.UpdateRestaurant(restaurant);
            });
        }
    }
}