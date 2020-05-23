using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Data;
using wheredoyouwanttoeat2.Classes;

namespace wheredoyouwanttoeat2.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected ApplicationDbContext _db;

        public BaseController(UserManager<User> manager, ApplicationDbContext dbContext)
        {
            _userManager = manager;
            _db = dbContext;
        }

        protected Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
