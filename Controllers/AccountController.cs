using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wheredoyouwanttoeat2.Models;
using System.Threading.Tasks;

namespace wheredoyouwanttoeat2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
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

        protected Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}