using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using wheredoyouwanttoeat2.ViewModel;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Data;
using wheredoyouwanttoeat2.Classes;
namespace wheredoyouwanttoeat2.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<User> manager, ApplicationDbContext dbContext) : base(manager, dbContext)
        {

        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser = await GetCurrentUserAsync();
            var viewModel = new Randomizer
            {
                RestaurantCount = 0,
                Tags = new List<Tag>(),
                ChoiceCount = 0,
                ButtonText = "Choose Where to Eat!",
                SelectedRestaurant = null,
                ErrorText = string.Empty
            };

            if (loggedInUser != null)
            {
                TempData["choice_count"] = 0;

                var tags = _db.RestaurantTags.Where(rt => rt.Restaurant.UserId == loggedInUser.Id).Select(rt => rt.Tag).ToList();

                viewModel.RestaurantCount = _db.Restaurants.Count(r => r.UserId == loggedInUser.Id);
                viewModel.Tags = tags;

                return View(viewModel);
            }

            return View(viewModel);
        }

        [Route("privacy-policy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Randomizer model)
        {
            var loggedInUser = await GetCurrentUserAsync();

            int choiceCount = 1;
            if (TempData["choice_count"] != null)
            {
                choiceCount = (int)TempData["choice_count"] + 1;
            }

            TempData["choice_count"] = choiceCount;

            model.RestaurantCount = _db.Restaurants.Count(r => r.UserId == loggedInUser.Id);
            var userTagsCount = _db.RestaurantTags.Count(rt => rt.Restaurant.UserId == loggedInUser.Id);

            List<Restaurant> restaurants = new List<Restaurant>();

            if (userTagsCount == 0)
            {
                // user doesn't have any tags, just randomly choose a restaurant
                restaurants = _db.Restaurants.Where(r => r.UserId == loggedInUser.Id).ToList();
            }
            else
            {
                if (model.Tags.Count(t => t.Selected) > 0)
                {
                    List<int> selectedTags = model.Tags.Where(t => t.Selected).Select(t => t.TagId).ToList();
                    restaurants = _db.RestaurantTags.Where(rt => selectedTags.Contains(rt.TagId)).Select(rt => rt.Restaurant).ToList();
                }
                else
                {
                    // user has tags, but has de-selected everything, alert them
                    model.ErrorText = "Please select at least one tag";
                    return View(model);
                }
            }

            Random rnd = new Random();
            int index = rnd.Next(0, restaurants.Count);

            model.SelectedRestaurant = restaurants[index];

            if (choiceCount <= 4)
            {
                model.LeadingText = "Looks like you're going to...";
            }
            else if (choiceCount > 4 && choiceCount <= 8)
            {
                index = rnd.Next(0, Utilities.TooManyChoices.Count);
                model.LeadingText = Utilities.TooManyChoices[index];
            }
            else if (choiceCount > 8 && choiceCount <= 20)
            {
                index = rnd.Next(0, Utilities.WayTooManyChoices.Count);
                model.LeadingText = Utilities.WayTooManyChoices[index];
            }
            else
            {
                index = rnd.Next(0, Utilities.WayWayTooManyChoices.Count);
                model.LeadingText = Utilities.WayWayTooManyChoices[index];
            }

            model.ButtonText = "Meh...Choose Another";
            model.ChoiceCount = choiceCount;
            model.ErrorText = string.Empty;

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
