using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Data;
using Microsoft.Extensions.Logging;

namespace wheredoyouwanttoeat2.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<User> _userManager;
        protected ApplicationDbContext _db;
        protected readonly ILogger _logger;

        public BaseController(UserManager<User> manager, ApplicationDbContext dbContext, ILogger<BaseController> logger)
        {
            _userManager = manager;
            _db = dbContext;
            _logger = logger;
        }

        protected Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
