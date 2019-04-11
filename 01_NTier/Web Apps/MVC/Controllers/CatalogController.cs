using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using MVC.Model.UserData;
using Services;
using System;
using System.Threading.Tasks;

namespace Controllers
{
    public class CatalogController : BaseController
    {
        private readonly ICatalogService catalogService;

        public CatalogController
            (ICatalogService catalogService,
            IUserRedisRepository repository)
            : base(repository)
        {
            this.catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            await CheckUserCounterData();
            var products = await catalogService.GetProducts();
            var resultado = new SearchProductsViewModel(products, "");
            return base.View(resultado);
        }

        public async Task<IActionResult> SearchProducts(string search)
        {
            await CheckUserCounterData();
            var products = await catalogService.SearchProducts(search);
            var resultado = new SearchProductsViewModel(products, search);
            return View("Index", resultado);
        }
    }
}

