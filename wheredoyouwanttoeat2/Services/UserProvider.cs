using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;

namespace wheredoyouwanttoeat2.Services
{
    public class UserProvider : IUserProvider
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<User> _signInManager;

        public UserProvider(IHttpContextAccessor contextAccessor, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string GetUserId()
        {
            return _userManager.GetUserId(_contextAccessor.HttpContext.User);
        }

        public async Task<User> GetLoggedInUserAsync()
        {
            return await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}