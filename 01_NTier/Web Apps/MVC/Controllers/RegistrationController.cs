using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using MVC;
using MVC.Model.UserData;
using Services;
using Services.Models;
using System.Threading.Tasks;

namespace Controllers
{
    public class RegistrationController : BaseController
    {
        public RegistrationController(
            IUserRedisRepository repository)
            : base(repository)
        {
        }

        public async Task<IActionResult> Index()
        {
            var usuario = UserManager.GetUser();

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

            await CheckUserCounterData();
            return View(registration);
        }
    }
}
