using Microsoft.AspNetCore.Mvc;
using wheredoyouwanttoeat2.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace wheredoyouwanttoeat2.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly IUserProvider _userProvider;

        public BaseController(IUserProvider provider, ILogger<BaseController> logger)
        {
            _userProvider = provider;
            _logger = logger;
        }
    }
}
