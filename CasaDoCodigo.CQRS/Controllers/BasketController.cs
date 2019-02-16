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
        private readonly ICatalogoService catalogoService;
        private readonly IBasketService basketService;

        public BasketController(
            IHttpContextAccessor contextAccessor,
            IIdentityParser<ApplicationUser> appUserParser,
            ILogger<BasketController> logger,
            ICatalogoService catalogoService,
            IBasketService basketService,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.appUserParser = appUserParser;
            this.catalogoService = catalogoService;
            this.basketService = basketService;
        }

        public async Task<IActionResult> Index(string codigo = null)
        {
            await CheckUserNotificationCount();

            try
            {
                string idUsuario = GetUserId();

                BasketCliente basket;
                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    var produto = await catalogoService.GetProduto(codigo);
                    if (produto == null)
                    {
                        return RedirectToAction("ProdutoNaoEncontrado", "Basket", codigo);
                    }

                    ItemBasket itemBasket = new ItemBasket(produto.Codigo, produto.Codigo, produto.Nome, produto.Preco, 1, produto.UrlImagem);
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
                HandleBrokenCircuitException(catalogoService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }

        public IActionResult ProdutoNaoEncontrado(string codigo)
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
        public async Task<IActionResult> UpdateQuantidade([FromBody]UpdateQuantidadeInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UpdateQuantidadeOutput value = await basketService.UpdateItem(GetUserId(), input);
            if (value == null)
            {
                return NotFound(input);
            }

            return base.Ok(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Cadastro cadastro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var viewModel = new CadastroViewModel(cadastro);
                    await basketService.Checkout(GetUserId(), viewModel);
                    return RedirectToAction("Checkout");
                }
                return RedirectToAction("Index", "Cadastro");
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
                return View(new PedidoConfirmado(usuario.Email));
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