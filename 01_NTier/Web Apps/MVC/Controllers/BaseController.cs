using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC;
using MVC.Model.UserData;
using Services;
using System.Threading.Tasks;

namespace Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger logger;
        protected readonly IUserRedisRepository userRedisRepository;

        protected BaseController(ILogger logger, IUserRedisRepository repository)
        {
            this.logger = logger;
            this.userRedisRepository = repository;
        }

        protected string GetUserId()
        {
            return UserManager.GetUser().Id;
        }

        protected async Task CheckUserCounterData()
        {
            var user = UserManager.GetUser();
            ViewBag.UserCounterData
                = userRedisRepository.GetUserCounterData(user.Id);
        }
    }
}
