using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wheredoyouwanttoeat2.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using wheredoyouwanttoeat2.Services.Interfaces;
using System;

namespace wheredoyouwanttoeat2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAccountService _service;
        private readonly IUserProvider _userProvider;

        public AccountController(IUserProvider provider, IAccountService service, ILogger<AccountController> logger)
        {
            _userProvider = provider;
            _service = service;
            _logger = logger;
        }

        [AllowAnonymous]
        [Route("register")]
        public IActionResult Register()
        {
            var model = new ViewModel.Register();
            return View(model);
        }

        [AllowAnonymous]
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

                    var result = await _service.RegisterUserAsync(user, model.Password);

                    if (result)
                    {
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

        [AllowAnonymous]
        [Route("login/{ReturnUrl?}")]
        public IActionResult Login(string ReturnUrl = "")
        {
            var model = new ViewModel.Login();
            model.ReturnUrl = ReturnUrl;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<IActionResult> Login(ViewModel.Login model)
        {
            model.ClearMessages();

            if (ModelState.IsValid)
            {
                try
                {
                    bool loginSuccessful = await _service.LoginUserAsync(model.Email, model.Password);
                    if (loginSuccessful)
                    {
                        if (model.ReturnUrl != null && model.ReturnUrl != "")
                        {
                            return Redirect(model.ReturnUrl);
                        }

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
                    _logger.LogError(ex, $"Error logging in user: {model.Email}");
                    model.ErrorMessage = "Invalid email address or password";
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("edit-profile")]
        public async Task<IActionResult> EditProfile()
        {
            var loggedInUser = await _userProvider.GetLoggedInUserAsync();

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
                    var result = await _service.UpdateUserProfileAsync(model.Email, model.Name);

                    if (result)
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
                    var result = await _service.ChangeUserPasswordAsync(model.CurrentPassword, model.Password);

                    if (result)
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
                var loggedInUser = await _userProvider.GetLoggedInUserAsync();

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

                var restaurants = _service.GetUserRestaurants().OrderBy(r => r.Name).ToList();

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
                    var errorMessage = await _service.DeleteUserAccountAsync(model.Password);

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
    }
}