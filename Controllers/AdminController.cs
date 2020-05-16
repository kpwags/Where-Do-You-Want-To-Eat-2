using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using wheredoyouwanttoeat2.Data;
using wheredoyouwanttoeat2.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace wheredoyouwanttoeat2.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> manager, ILogger<AdminController> logger, ApplicationDbContext dbContext)
        {
            _userManager = manager;
            _logger = logger;
            _db = dbContext;
        }

        [Route("/admin/restaurants")]
        public async Task<IActionResult> Restaurants()
        {
            var loggedInUser = await GetCurrentUserAsync();
            var model = _db.Restaurants.Where(r => r.UserId == loggedInUser.Id).OrderBy(r => r.Name).ToList();
            return View(model);
        }

        [Route("/admin/add-restaurant")]
        public IActionResult AddRestaurant()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRestaurant(Restaurant model)
        {
            if (ModelState.IsValid)
            {
                var restaurant = new Restaurant
                {
                    Name = model.Name,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode
                };

                _db.Restaurants.Add(restaurant);
                await _db.SaveChangesAsync();

                return RedirectToAction("Restaurants", "Admin");
            }

            return View();
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}