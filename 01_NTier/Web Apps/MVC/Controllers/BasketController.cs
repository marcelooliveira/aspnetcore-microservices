using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using MVC;
using MVC.Model.UserData;
using Services;
using Services.Models;
using System;
using System.Threading.Tasks;

namespace Controllers
{
    public class BasketController : BaseController
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public BasketController(
            IHttpContextAccessor contextAccessor,
            ICatalogService catalogService,
            IBasketService basketService,
            IUserRedisRepository repository)
            : base(repository)
        {
            this.catalogService = catalogService;
            this.basketService = basketService;
        }

        public async Task<IActionResult> Index(string code = null)
        {
            await CheckUserCounterData();

            string idUsuario = GetUserId();

            CustomerBasket basket;
            if (!string.IsNullOrWhiteSpace(code))
            {
                var product = await catalogService.GetProduct(code);
                if (product == null)
                {
                    return RedirectToAction("ProductNotFound", "Basket", code);
                }

                BasketItem itemBasket = new BasketItem(product.Code, product.Code, product.Name, product.Price, 1);
                basket = basketService.AddItem(idUsuario, itemBasket);
            }
            else
            {
                basket = basketService.GetBasket(idUsuario);
            }
            await CheckUserCounterData();
            return View(basket);

        }

        async Task<CustomerBasket> AddProductAsync(string code = null)
        {
            string idUsuario = GetUserId();
            CustomerBasket basket;
            if (!string.IsNullOrWhiteSpace(code))
            {
                var product = await catalogService.GetProduct(code);
                if (product == null)
                {
                    return null;
                }

                BasketItem itemBasket =
                    new BasketItem(product.Code
                    , product.Code
                    , product.Name
                    , product.Price
                    , 1);
                basket = basketService.AddItem(idUsuario, itemBasket);
                await CheckUserCounterData();
            }
            else
            {
                basket = basketService.GetBasket(idUsuario);
            }
            return basket;
        }

        public IActionResult ProductNotFound(string code)
        {
            return View(code);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity([FromBody]UpdateQuantityInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UpdateQuantityOutput value = basketService.UpdateItem(GetUserId(), input);
            if (value == null)
            {
                return NotFound(input);
            }

            return base.Ok(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Registration registration)
        {
            if (ModelState.IsValid)
            {
                var viewModel = new RegistrationViewModel(registration);
                await basketService.CheckoutAsync(GetUserId(), viewModel);
                return RedirectToAction("Checkout");
            }
            return RedirectToAction("Index", "Registration");
        }

        public async Task<IActionResult> Checkout()
        {
            string idUsuario = GetUserId();

            var usuario = UserManager.GetUser();

            await CheckUserCounterData();
            return View(new OrderConfirmed(usuario.Email));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToBasket([FromBody]string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return NotFound(code);
            }

            CustomerBasket basket = await AddProductAsync(code);

            return base.Ok(basket);
        }
    }
}