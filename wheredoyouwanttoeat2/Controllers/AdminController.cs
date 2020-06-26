using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using wheredoyouwanttoeat2.Classes;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace wheredoyouwanttoeat2.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private readonly IAdminService _service;

        public AdminController(IUserProvider provider, ILogger<AdminController> logger, IAdminService service) : base(provider, logger)
        {
            _service = service;
        }

        [Route("/admin/restaurants")]
        public async Task<IActionResult> Restaurants()
        {
            try
            {
                var loggedInUser = await _userProvider.GetLoggedInUserAsync();

                var model = new ViewModel.RestaurantAdmin
                {
                    Restaurants = _service.GetUserRestaurants(loggedInUser.Id).OrderBy(r => r.Name).ToList()
                };

                if (TempData["errormessage"] != null)
                {
                    model.ErrorMessage = TempData["errormessage"].ToString();
                    TempData["errormessage"] = null;
                }

                foreach (var restaurant in model.Restaurants)
                {
                    restaurant.TagString = string.Join(", ", restaurant.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId).Select(rt => rt.Tag.Name).ToList());
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving restaurants");
                var model = new ViewModel.RestaurantAdmin
                {
                    ErrorMessage = "Error retrieiving restaurants"
                };
                return View(model);
            }
        }

        [Route("/admin/add-restaurant")]
        public IActionResult AddRestaurant()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("/admin/add-restaurant")]
        public async Task<IActionResult> AddRestaurant(Restaurant model)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = await _userProvider.GetLoggedInUserAsync();

                Restaurant restaurant;

                try
                {
                    restaurant = new Restaurant
                    {
                        Name = model.Name,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        City = model.City,
                        State = model.State,
                        ZipCode = model.ZipCode,
                        PhoneNumber = model.PhoneNumber,
                        Website = model.Website,
                        Menu = model.Menu,
                        Latitude = 0,
                        Longitude = 0,
                        User = loggedInUser,
                        UserId = loggedInUser.Id
                    };

                    if (restaurant == null)
                    {
                        Console.WriteLine("new restaurant is null");
                    }

                    restaurant = await _service.AddRestaurant(restaurant);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving restaurant");
                    return View();
                }

                if (restaurant == null)
                {
                    // just in case
                    return RedirectToAction("Restaurants", "Admin");
                }

                if (model.TagString != null && model.TagString.Trim().Length > 0)
                {
                    try
                    {
                        await _service.AddTagsToRestaurant(restaurant, model.TagString.Split(',').ToList());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving tags for restaurant");
                    }
                }

                return RedirectToAction("Restaurants", "Admin");
            }

            return View();
        }

        [Route("/admin/edit-restaurant/{id:int}")]
        public IActionResult EditRestaurant(int id)
        {
            var restaurant = _service.GetRestaurantById(id);

            if (restaurant != null)
            {
                try
                {
                    restaurant.TagString = string.Join(',', _service.GetRestaurantTags(id).Select(rt => rt.Tag.Name).ToList());
                }
                catch (Exception ex)
                {
                    restaurant.TagString = string.Empty;
                    _logger.LogError(ex, "Error generating tag string");
                }

                return View(restaurant);
            }

            // restaurant not found, redirect to list
            TempData["errormessage"] = "Restaurant not found";
            return RedirectToAction("Restaurants");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("/admin/edit-restaurant/{id:int}")]
        public async Task<IActionResult> EditRestaurant(Restaurant model)
        {
            if (@ModelState.IsValid)
            {
                var restaurant = _service.GetRestaurantById(model.RestaurantId);

                if (restaurant == null)
                {
                    // restaurant not found, redirect to list
                    TempData["errormessage"] = "Restaurant not found";
                    return RedirectToAction("Restaurants");
                }

                // first, let's handle the tags
                List<string> restaurantTags = Utilities.CorrectUserEnteredTags(model.TagString);

                try
                {
                    await _service.UpdateRestaurantTags(restaurant, restaurantTags);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving tags while updating a restaurant");
                }

                try
                {
                    await _service.CleanUpTags(restaurant, restaurantTags);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting unused tags while updating a restaurant");
                }

                bool hasAddressChanged = Restaurant.HasAddressChanged(restaurant, model);

                // now that the tags are cleaned up, let's update the restaurant
                restaurant.Name = model.Name;
                restaurant.AddressLine1 = model.AddressLine1;
                restaurant.AddressLine2 = model.AddressLine2;
                restaurant.City = model.City;
                restaurant.State = model.State;
                restaurant.ZipCode = model.ZipCode;
                restaurant.PhoneNumber = model.PhoneNumber;
                restaurant.Website = model.Website;
                restaurant.Menu = model.Menu;

                try
                {
                    await _service.UpdateRestaurant(restaurant, hasAddressChanged);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating restaurant");
                }

                return RedirectToAction("Restaurants", "Admin");
            }

            return View(model);
        }

        [Route("/admin/delete-restaurant/{id:int}")]
        public IActionResult DeleteRestaurant(int id)
        {
            var restaurant = _service.GetRestaurantById(id);

            if (restaurant != null)
            {
                return View(restaurant);
            }

            // restaurant not found, redirect to list
            TempData["errormessage"] = "Restaurant not found";
            return RedirectToAction("Restaurants");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("/admin/delete-restaurant/{id:int}")]
        public async Task<IActionResult> DeleteRestaurant(Restaurant model)
        {
            var restaurant = _service.GetRestaurantById(model.RestaurantId);

            string errorMessage = string.Empty;

            if (restaurant == null)
            {
                // restaurant not found, redirect to list
                TempData["errormessage"] = "Restaurant not found";
                return RedirectToAction("Restaurants");
            }

            try
            {
                await _service.DeleteRestaurant(restaurant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting restaurant");
                TempData["errormessage"] = "Error deleting restaurant";
                return RedirectToAction("Restaurants");
            }

            return RedirectToAction("Restaurants");
        }
    }
}