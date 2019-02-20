using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.Models;
using MVC.SignalR;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    [Authorize]
    public class BasketController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public BasketController(
            IHttpContextAccessor contextAccessor,
            IIdentityParser<ApplicationUser> appUserParser,
            ILogger<BasketController> logger,
            ICatalogService catalogService,
            IBasketService basketService,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.appUserParser = appUserParser;
            this.catalogService = catalogService;
            this.basketService = basketService;
        }

        public async Task<IActionResult> Index(string codigo = null)
        {
            await CheckUserNotificationCount();

            try
            {
                string idUsuario = GetUserId();

                CustomerBasket basket;
                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    var product = await catalogService.GetProduct(codigo);
                    if (product == null)
                    {
                        return RedirectToAction("ProductNotFound", "Basket", codigo);
                    }

                    BasketItem itemBasket = new BasketItem(product.Code, product.Code, product.Name, product.Price, 1, product.ImageURL);
                    basket = await basketService.AddItem(idUsuario, itemBasket);
                }
                else
                {
                    basket = await basketService.GetBasket(idUsuario);
                }
                return View(basket);
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(catalogService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }

        public IActionResult ProductNotFound(string codigo)
        {
            return View(codigo);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantidades, string action)
        {
            try
            {
                var usuario = appUserParser.Parse(HttpContext.User);
                var basket = basketService.DefinirQuantidades(usuario, quantidades);
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(basketService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateQuantity([FromBody]UpdateQuantityInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UpdateQuantityOutput value = await basketService.UpdateItem(GetUserId(), input);
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
            try
            {
                if (ModelState.IsValid)
                {
                    var viewModel = new RegistrationViewModel(registration);
                    await basketService.Checkout(GetUserId(), viewModel);
                    return RedirectToAction("Checkout");
                }
                return RedirectToAction("Index", "Registration");
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(basketService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            await CheckUserNotificationCount();

            try
            {
                string idUsuario = GetUserId();

                var usuario = appUserParser.Parse(HttpContext.User);
                return View(new OrderConfirmed(usuario.Email));
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