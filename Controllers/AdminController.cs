using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using wheredoyouwanttoeat2.Classes;
using wheredoyouwanttoeat2.Data;
using wheredoyouwanttoeat2.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace wheredoyouwanttoeat2.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<User> manager, ApplicationDbContext dbContext) : base(manager, dbContext)
        {

        }

        [Route("/admin/restaurants")]
        public async Task<IActionResult> Restaurants()
        {
            var loggedInUser = await GetCurrentUserAsync();
            var model = _db.Restaurants.Where(r => r.UserId == loggedInUser.Id).OrderBy(r => r.Name).ToList();

            foreach (var restaurant in model)
            {
                if (restaurant.RestaurantTags == null)
                {
                    restaurant.TagString = string.Join(", ", _db.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId).Select(rt => rt.Tag.Name).ToList());
                }
                else
                {
                    restaurant.TagString = string.Join(", ", restaurant.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId).Select(rt => rt.Tag.Name).ToList());
                }
            }

            return View(model);
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
                var loggedInUser = await GetCurrentUserAsync();

                var restaurant = new Restaurant
                {
                    Name = model.Name,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                    User = loggedInUser,
                    UserId = loggedInUser.Id
                };

                _db.Restaurants.Add(restaurant);
                await _db.SaveChangesAsync();

                if (model.TagString != null && model.TagString.Trim().Length > 0)
                {
                    foreach (string tag in model.TagString.Split(',').ToList())
                    {
                        var dbTag = _db.Tags.Where(t => t.Name.ToLower() == tag.ToLower()).FirstOrDefault();
                        if (dbTag == null)
                        {
                            // tag does not exist, add it
                            dbTag = new Tag
                            {
                                Name = tag.Trim().ToLower()
                            };

                            _db.Tags.Add(dbTag);
                            await _db.SaveChangesAsync();
                        }

                        // create the restaurant-tag link
                        var restaurantTag = new RestaurantTag
                        {
                            Restaurant = restaurant,
                            RestaurantId = restaurant.RestaurantId,
                            Tag = dbTag,
                            TagId = dbTag.TagId
                        };
                        _db.RestaurantTags.Add(restaurantTag);
                        await _db.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Restaurants", "Admin");
            }

            return View();
        }

        [Route("/admin/edit-restaurant/{id:int}")]
        public IActionResult EditRestaurant(int id)
        {
            var restaurant = _db.Restaurants.Where(r => r.RestaurantId == id).FirstOrDefault();

            if (restaurant != null)
            {
                List<string> tags = _db.RestaurantTags.Where(rt => rt.RestaurantId == id).Select(rt => rt.Tag.Name).ToList();
                restaurant.TagString = string.Join(',', tags);

                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Restaurants");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("/admin/edit-restaurant/{id:int}")]
        public async Task<IActionResult> EditRestaurant(Restaurant model)
        {
            if (@ModelState.IsValid)
            {
                var restaurant = _db.Restaurants.Where(r => r.RestaurantId == model.RestaurantId).FirstOrDefault();

                // first, let's handle the tags
                List<string> restaurantTags = Utilities.CorrectUserEnteredTags(model.TagString);
                foreach (string tag in restaurantTags)
                {
                    var dbTag = _db.Tags.Where(t => t.Name.ToLower() == tag.ToLower()).FirstOrDefault();
                    if (dbTag == null)
                    {
                        // tag does not exist, add it
                        dbTag = new Tag
                        {
                            Name = tag.Trim().ToLower()
                        };

                        _db.Tags.Add(dbTag);
                        await _db.SaveChangesAsync();
                    }

                    // check to see if the link exists
                    var dbRestaurantTag = _db.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId && rt.TagId == dbTag.TagId).FirstOrDefault();
                    if (dbRestaurantTag == null)
                    {
                        // link does not exist, create it
                        var restaurantTag = new RestaurantTag
                        {
                            Restaurant = restaurant,
                            RestaurantId = restaurant.RestaurantId,
                            Tag = dbTag,
                            TagId = dbTag.TagId
                        };
                        _db.RestaurantTags.Add(restaurantTag);
                        await _db.SaveChangesAsync();
                    }
                }

                List<int> restaurantTagsToDelete = new List<int>();
                foreach (var rt in _db.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId))
                {
                    var tag = _db.Tags.Where(t => t.TagId == rt.TagId).FirstOrDefault();
                    if (tag != null && !restaurantTags.Contains(tag.Name))
                    {
                        // add it to the list to delete
                        restaurantTagsToDelete.Add(rt.TagId);
                    }
                }

                foreach (int tagId in restaurantTagsToDelete)
                {
                    var tagToDelete = _db.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId && rt.TagId == tagId).FirstOrDefault();
                    if (tagToDelete != null)
                    {
                        _db.RestaurantTags.Remove(tagToDelete);
                        await _db.SaveChangesAsync();
                    }
                }

                // check to see if the tag is used anywhere else
                List<int> tagsToDelete = new List<int>();
                foreach (int tagId in restaurantTagsToDelete)
                {
                    if (_db.RestaurantTags.Where(rt => rt.TagId == tagId).Count() == 0)
                    {
                        // it's not used anywhere else, let's delete it to keep the database cleaner
                        tagsToDelete.Add(tagId);
                    }
                }

                foreach (int tagId in tagsToDelete)
                {
                    var tagToDelete = _db.Tags.Where(t => t.TagId == tagId).FirstOrDefault();
                    if (tagToDelete != null)
                    {
                        _db.Tags.Remove(tagToDelete);
                        await _db.SaveChangesAsync();
                    }
                }

                // now that the tags are cleaned up, let's update the restaurant
                restaurant.Name = model.Name;
                restaurant.AddressLine1 = model.AddressLine1;
                restaurant.AddressLine2 = model.AddressLine2;
                restaurant.City = model.City;
                restaurant.State = model.State;
                restaurant.ZipCode = model.ZipCode;

                _db.Update(restaurant);
                await _db.SaveChangesAsync();

                return RedirectToAction("Restaurants", "Admin");
            }

            return View(model);
        }

        [Route("/admin/delete-restaurant/{id:int}")]
        public IActionResult DeleteRestaurant(int id)
        {
            var restaurant = _db.Restaurants.Where(r => r.RestaurantId == id).FirstOrDefault();

            if (restaurant != null)
            {
                return View(restaurant);
            }
            else
            {
                return RedirectToAction("Restaurants");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("/admin/delete-restaurant/{id:int}")]
        public async Task<IActionResult> DeleteRestaurant(Restaurant model)
        {
            var restaurant = _db.Restaurants.Where(r => r.RestaurantId == model.RestaurantId).FirstOrDefault();

            List<RestaurantTag> restaurantTags = _db.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId).ToList();
            List<int> tagIds = new List<int>();
            foreach (RestaurantTag restaurantTag in restaurantTags)
            {
                // save to list for possible cleanup
                tagIds.Add(restaurantTag.TagId);

                _db.RestaurantTags.Remove(restaurantTag);
                await _db.SaveChangesAsync();
            }

            List<int> tagsToDelete = new List<int>();
            foreach (int tagId in tagIds)
            {
                if (_db.RestaurantTags.Where(rt => rt.TagId == tagId).Count() == 0)
                {
                    // it's not used anywhere else, let's delete it to keep the database cleaner
                    tagsToDelete.Add(tagId);
                }
            }

            // delete unused tags
            foreach (int tagId in tagsToDelete)
            {
                var tagToDelete = _db.Tags.Where(t => t.TagId == tagId).FirstOrDefault();
                if (tagToDelete != null)
                {
                    _db.Tags.Remove(tagToDelete);
                    await _db.SaveChangesAsync();
                }
            }

            _db.Restaurants.Remove(restaurant);
            await _db.SaveChangesAsync();

            return RedirectToAction("Restaurants");
        }
    }
}