using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wheredoyouwanttoeat2.Data;
using wheredoyouwanttoeat2.ViewModel;
using wheredoyouwanttoeat2.Models;
using Microsoft.Extensions.Logging;
using wheredoyouwanttoeat2.Classes;
using wheredoyouwanttoeat2.Services.Interfaces;

namespace wheredoyouwanttoeat2.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomeService _service;

        public HomeController(IUserProvider provider, ILogger<HomeController> logger, IHomeService service) : base(provider, logger)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userProvider.GetLoggedInUserAsync();
            var viewModel = new Randomizer
            {
                RestaurantCount = 0,
                Tags = new List<Tag>(),
                ChoiceCount = 0,
                ButtonText = "Choose Where to Eat!",
                SelectedRestaurant = null,
                ErrorMessage = string.Empty
            };

            if (loggedInUser != null)
            {
                try
                {
                    TempData["choice_count"] = 0;

                    var tags = _service.GetUserTags(loggedInUser.Id).ToList();

                    viewModel.RestaurantCount = _service.GetUserRestaurants(loggedInUser.Id).Count();
                    viewModel.Tags = tags;
                }
                catch (Exception ex)
                {
                    viewModel.ErrorMessage = "Error retrieving tags";
                    _logger.LogError(ex, $"Error initializng user tags for {loggedInUser.Email}");
                }

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
            var loggedInUser = await _userProvider.GetLoggedInUserAsync();

            model.ClearMessages();

            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                int choiceCount = 1;
                if (TempData["choice_count"] != null)
                {
                    choiceCount = (int)TempData["choice_count"] + 1;
                }

                TempData["choice_count"] = choiceCount;

                model.RestaurantCount = _service.GetUserRestaurants(loggedInUser.Id).Count();
                var userTagsCount = _service.GetUserTags(loggedInUser.Id).Count();

                List<Restaurant> restaurants = new List<Restaurant>();

                if (userTagsCount == 0)
                {
                    // user doesn't have any tags, just randomly choose a restaurant
                    restaurants = _service.GetUserRestaurants(loggedInUser.Id).ToList();
                }
                else
                {
                    if (model.Tags.Count(t => t.Selected) > 0)
                    {
                        List<int> selectedTags = model.Tags.Where(t => t.Selected).Select(t => t.TagId).ToList();
                        restaurants = _service.GetUserRestaurantsWithTags(selectedTags, loggedInUser.Id).ToList();
                    }
                    else
                    {
                        // user has tags, but has de-selected everything, alert them
                        model.ErrorMessage = "Please select at least one tag";
                        model.ButtonText = "Choose Where to Eat!";
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
                model.ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                model.ChoiceCount = 1;
                model.ErrorMessage = "Error picking restaurant";
                model.ButtonText = "Choose Where to Eat!";
                model.SelectedRestaurant = null;
                _logger.LogError(ex, "Error picking restaurant");
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
