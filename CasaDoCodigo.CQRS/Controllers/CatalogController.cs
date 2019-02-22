using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.SignalR;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CatalogController : BaseController
    {
        private readonly ICatalogService catalogService;

        public CatalogController
            (ICatalogService catalogService,
            ILogger<CatalogController> logger,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            await CheckUserNotificationCount();

            try
            {
                var products = await catalogService.GetProducts();
                var resultado = new SearchProductsViewModel(products, "");
                return base.View(resultado);
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

        public async Task<IActionResult> BuscaProducts(string pesquisa)
        {
            await CheckUserNotificationCount();

            try
            {
                var products = await catalogService.SearchProducts(pesquisa);
                var resultado = new SearchProductsViewModel(products, pesquisa);
                return View("Index", resultado);
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

            return View("Index");
        }

    }
}

