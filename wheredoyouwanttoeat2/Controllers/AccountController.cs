using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wheredoyouwanttoeat2.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using wheredoyouwanttoeat2.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using wheredoyouwanttoeat2.Services.Interfaces;
using System;

namespace wheredoyouwanttoeat2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAccountService _service;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IAccountService service, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _service = service;
            _logger = logger;
        }

        [Route("register")]
        public IActionResult Register()
        {
            var model = new ViewModel.Register();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<IActionResult> Register(ViewModel.Register model)
        {
            model.ClearMessages();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Name = model.Name,
                        UserName = model.Email,
                        Email = model.Email,
                    };

                    var result = await _service.RegisterUser(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = $"Error registerring user: {ex.Message}";
                    _logger.LogError(ex, "Error registerring user");
                }
            }

            return View(model);
        }

        [Route("login")]
        public IActionResult Login()
        {
            var model = new ViewModel.Login();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> Login(ViewModel.Login model)
        {
            model.ClearMessages();

            if (ModelState.IsValid)
            {
                try
                {
                    bool successfulLogin = await _service.LoginUser(model.Email, model.Password);

                    if (successfulLogin)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogInformation($"Failed login attempt for {model.Email} (IP: {HttpContext.Connection.RemoteIpAddress})");
                    }


                    model.ErrorMessage = "Invalid email address or password";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error loggin in user: {model.Email}");
                    model.ErrorMessage = "Invalid email address or password";
                }
            }

            return View(model);
        }

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("edit-profile")]
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
        [Route("edit-profile")]
        public async Task<IActionResult> EditProfile(ViewModel.EditProfile model)
        {
            model.ClearMessages();

            if (ModelState.IsValid)
            {
                try
                {
                    var loggedInUser = await GetCurrentUserAsync();

                    loggedInUser.Email = model.Email;
                    loggedInUser.Name = model.Name;

                    var result = await _service.UpdateUserProfile(loggedInUser);

                    if (result.Succeeded)
                    {
                        model.SuccessMessage = "Profile updated successfully";
                    }
                    else
                    {
                        model.ErrorMessage = "Error updating user profile";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error saving profile changes for {model.Email}");
                    model.ErrorMessage = "Error updating user profile";
                }
            }

            return View(model);
        }

        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            var model = new ViewModel.ChangePassword();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword(ViewModel.ChangePassword model)
        {
            model.ClearMessages();

            if (ModelState.IsValid)
            {
                try
                {
                    var loggedInUser = await GetCurrentUserAsync();

                    var result = await _service.ChangeUserPassword(loggedInUser, model.CurrentPassword, model.Password);

                    if (result.Succeeded)
                    {
                        model.SuccessMessage = "Password changed successfully";
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CurrentPassword), "Incorrect password");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error changing password");
                    model.ErrorMessage = "Error changing password";
                }
            }

            return View(model);
        }

        [Route("download-data")]
        public IActionResult DownloadData()
        {
            var model = new ViewModel.DownloadData();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadUserData([FromQuery] string dataFormat)
        {
            try
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

                var restaurants = _service.GetUserRestaurants(loggedInUser.Id).OrderBy(r => r.Name).ToList();

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
                string filename = string.Empty;
                string contentType = "application/octet-stream";

                switch (dataFormat)
                {
                    case "XML":
                        download = UserData.DownloadAsXML();
                        filename = "wheredoyouwanttoeat.xml";
                        contentType = "text/xml";
                        break;

                    case "JSON":
                        download = UserData.DownloadAsJSON();
                        filename = "wheredoyouwanttoeat.json";
                        contentType = "application/json";
                        break;
                }

                byte[] bytes = Encoding.ASCII.GetBytes(download);

                var content = new System.IO.MemoryStream(bytes);

                return File(content, contentType, filename);
            }
            catch (System.Xml.XmlException ex)
            {
                _logger.LogError(ex, "Error Downloading Data as XML");
                return RedirectToAction("DownloadData");
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogError(ex, "Error Downloading Data as JSON");
                return RedirectToAction("DownloadData");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Downloading Data");
                return RedirectToAction("DownloadData");
            }
        }

        [Route("delete-account")]
        public IActionResult DeleteAccount()
        {
            var model = new ViewModel.DeleteAccount();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("delete-account")]
        public async Task<IActionResult> DeleteAccount(ViewModel.DeleteAccount model)
        {
            model.ClearMessages();

            if (ModelState.IsValid)
            {
                try
                {
                    var loggedInUser = await GetCurrentUserAsync();

                    var errorMessage = await _service.DeleteUserAccount(loggedInUser, model.Password);

                    switch (errorMessage)
                    {
                        case "Invalid password":
                            ModelState.AddModelError(nameof(model.Password), "Invalid password");
                            return View(model);

                        case "Error deleting account":
                            model.ErrorMessage = "Error deleting account";
                            return View(model);

                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Error deleting account";
                    _logger.LogError(ex, "Error Deleting Account");
                }
            }

            return View(model);
        }



        protected Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}