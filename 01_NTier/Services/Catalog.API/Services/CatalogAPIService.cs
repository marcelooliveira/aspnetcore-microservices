using Catalog.API.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Services
{
    public class CatalogAPIService : ICatalogAPIService
    {
        private readonly ILogger logger;
        private readonly IProductQueries productQueries;

        public CatalogAPIService(ILogger<CatalogAPIService> logger,
            IProductQueries productQueries)
        {
            this.logger = logger;
            this.productQueries = productQueries;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await productQueries.GetProductsAsync();
        }

        public async Task<Product> GetProduct(string code)
        {
            return await productQueries.GetProductAsync(code);
        }

        public async Task<IEnumerable<Product>> SearchProducts(string search)
        {
            return await productQueries.GetProductsAsync(search);
        }
    }
}