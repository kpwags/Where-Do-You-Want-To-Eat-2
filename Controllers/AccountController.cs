using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wheredoyouwanttoeat2.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using wheredoyouwanttoeat2.Data;
using System.Linq;

namespace wheredoyouwanttoeat2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        protected ApplicationDbContext _db;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._db = dbContext;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ViewModel.Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ViewModel.Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(nameof(model.Email), "Login Failed: User Not Found");

            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EditProfile()
        {
            var loggedInUser = await GetCurrentUserAsync();

            var editProfile = new ViewModel.EditProfile
            {
                Name = loggedInUser.Name,
                Email = loggedInUser.Email
            };

            return View(editProfile);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ViewModel.EditProfile model)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = await GetCurrentUserAsync();

                loggedInUser.Email = model.Email;
                loggedInUser.Name = model.Name;

                var result = await _userManager.UpdateAsync(loggedInUser);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(nameof(model.Email), "Error updating user profile");
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ViewModel.ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = await GetCurrentUserAsync();

                var result = await _userManager.ChangePasswordAsync(loggedInUser, model.CurrentPassword, model.Password);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword), "Incorrect password");
                }
            }

            return View(model);
        }

        public IActionResult DownloadData()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadData(ViewModel.DownloadData model)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = await GetCurrentUserAsync();

                var UserData = new Classes.DownloadData.UserData
                {
                    User = new Classes.DownloadData.User
                    {
                        Id = loggedInUser.Id,
                        Username = loggedInUser.UserName,
                        NormalizedUsername = loggedInUser.NormalizedUserName,
                        Email = loggedInUser.Email,
                        NormalizedEmail = loggedInUser.NormalizedEmail,
                        Name = loggedInUser.Name
                    },
                    Restaurants = new List<Classes.DownloadData.Restaurant>()
                };

                var restaurants = _db.Restaurants.Where(r => r.UserId == loggedInUser.Id).OrderBy(r => r.Name).ToList();

                foreach (var restaurant in restaurants)
                {
                    Classes.DownloadData.Restaurant r = new Classes.DownloadData.Restaurant
                    {
                        RestaurantId = restaurant.RestaurantId,
                        Name = restaurant.Name,
                        AddressLine1 = restaurant.AddressLine1,
                        AddressLine2 = restaurant.AddressLine2,
                        City = restaurant.City,
                        State = restaurant.State,
                        ZipCode = restaurant.ZipCode,
                        PhoneNumber = restaurant.PhoneNumber,
                        Website = restaurant.Website,
                        Menu = restaurant.Menu,
                        Latitude = restaurant.Latitude,
                        Longitude = restaurant.Longitude,
                        Tags = string.Join(", ", restaurant.RestaurantTags.Where(rt => rt.RestaurantId == restaurant.RestaurantId).Select(rt => rt.Tag.Name).ToList())
                    };

                    UserData.Restaurants.Add(r);
                }

                string download = string.Empty;
                switch (model.DataFormat)
                {
                    case "XML":
                        download = UserData.DownloadAsXML();
                        break;

                    case "JSON":
                        download = UserData.DownloadAsJSON();
                        break;

                    default:
                        ModelState.AddModelError(nameof(model.DataFormat), "Invalid data format");
                        break;
                }
            };
            return View(model);
        }

        protected Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}