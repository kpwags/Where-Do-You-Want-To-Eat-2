using Microsoft.AspNetCore.Mvc;
using WhereDoYouWantToEat2.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace WhereDoYouWantToEat2.Controllers
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
