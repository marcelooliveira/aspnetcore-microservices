using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using MVC.Model.UserData;
using Services;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers
{
    [Route("[controller]")]
    public class OrderController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;
        private readonly IOrderService orderService;

        public OrderController(
            IIdentityParser<ApplicationUser> appUserParser,
            IOrderService orderService,
            IUserRedisRepository repository) 
            : base(repository)
        {
            this.appUserParser = appUserParser;
            this.orderService = orderService;
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult> History(string customerId)
        {
            await CheckUserCounterData();

            List<OrderDTO> model = await orderService.GetAsync(customerId);
            return base.View(model);
        }

        [Route("Notifications")]
        public async Task<ActionResult> Notifications()
        {
            string customerId = GetUserId();
            UserCounterData userCounterData = userRedisRepository.GetUserCounterData(customerId);
            userRedisRepository.MarkAllAsRead(customerId);
            await CheckUserCounterData();
            return View(userCounterData);
        }
    }
}
