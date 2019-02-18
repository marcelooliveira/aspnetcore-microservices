using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.SignalR;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;

        public RegistrationController(
            IIdentityParser<ApplicationUser> appUserParser,
            ILogger<RegistrationController> logger,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.appUserParser = appUserParser;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            await CheckUserNotificationCount();

            try
            {
                var usuario = appUserParser.Parse(HttpContext.User);
                RegistrationViewModel registration
                    = new RegistrationViewModel()
                    {
                        District = usuario.District,
                        ZipCode = usuario.ZipCode,
                        AdditionalAddress = usuario.AdditionalAddress,
                        Email = usuario.Email,
                        Address = usuario.Address,
                        City = usuario.City,
                        Name = usuario.Name,
                        Phone = usuario.Phone,
                        State = usuario.State
                    };

                return View(registration);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }
    }
}
