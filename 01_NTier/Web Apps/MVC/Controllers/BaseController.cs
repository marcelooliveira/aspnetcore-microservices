using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.ViewModels;
using MVC;
using MVC.Model.Redis;
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

        protected void HandleBrokenCircuitException(IService service)
        {
            ViewBag.MsgServiceUnavailable = $"O serviço '{service.Scope}' não está ativo, por favor tente novamente mais tarde.";
        }

        protected void HandleException()
        {
            ViewBag.MsgServiceUnavailable = $"O serviço está indisponível no momento, por favor tente novamente mais tarde.";
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        protected string GetUserId()
        {
            return UserManager.GetUser().Id;
        }

        protected async Task CheckUserCounterData()
        {
            var user = UserManager.GetUser();
            ViewBag.UserCounterData
                = await userRedisRepository.GetUserCounterDataAsync(user.Id);
        }
    }
}
