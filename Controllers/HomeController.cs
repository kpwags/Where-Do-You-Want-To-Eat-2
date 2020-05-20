using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var viewModel = new ChooserViewModel
            {
                Tags = new List<Tag>(),
                ChoiceCount = 0,
                ButtonText = "Choose Where to Eat!",
                SelectedRestaurant = null
            };

            if (loggedInUser != null)
            {
                TempData["choice_count"] = 0;

                var tagIds = _db.RestaurantTags.Where(rt => rt.Restaurant.UserId == loggedInUser.Id).Select(rt => rt.TagId).ToList();
                var tags = _db.Tags.Where(t => tagIds.Contains(t.TagId)).OrderBy(t => t.Name).ToList();

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
        public async Task<IActionResult> Index(ChooserViewModel model)
        {
            var loggedInUser = await GetCurrentUserAsync();

            int choiceCount = 1;
            if (TempData["choice_count"] != null)
            {
                choiceCount = (int)TempData["choice_count"] + 1;
            }

            TempData["choice_count"] = choiceCount;

            List<int> selectedTags = model.Tags.Where(t => t.Selected).Select(t => t.TagId).ToList();

            List<int> restaurantIds = _db.RestaurantTags.Where(rt => selectedTags.Contains(rt.TagId)).Select(rt => rt.RestaurantId).ToList();
            var restaurants = _db.Restaurants.Where(r => restaurantIds.Contains(r.RestaurantId) && r.UserId == loggedInUser.Id).ToList();

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

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
