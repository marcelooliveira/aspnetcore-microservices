using Microsoft.AspNetCore.Mvc;
using MVC;
using MVC.Model.UserData;
using System.Threading.Tasks;

namespace Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IUserRedisRepository userRedisRepository;

        protected BaseController(IUserRedisRepository repository)
        {
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
