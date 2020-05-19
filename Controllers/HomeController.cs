using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Data;

namespace wheredoyouwanttoeat2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private ApplicationDbContext _db;

        public HomeController(UserManager<User> manager, ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _userManager = manager;
            _logger = logger;
            _db = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser = await GetCurrentUserAsync();

            if (loggedInUser != null)
            {
                var tagIds = _db.RestaurantTags.Where(rt => rt.Restaurant.UserId == loggedInUser.Id).Select(rt => rt.TagId).ToList();
                var tags = _db.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();

                return View(tags);
            }

            return View(new List<Tag>());
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
