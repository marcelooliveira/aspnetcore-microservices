using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public class CatalogService : BaseHttpService, ICatalogService
    {
        class ApiUris
        {
            public static string GetProduct => "api/product";
            public static string SearchProducts => "api/search";
        }

        private readonly ILogger<CatalogService> _logger;

        public CatalogService(
            IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper
            , ILogger<CatalogService> logger)
            : base(configuration, httpClient, sessionHelper)
        {
            _logger = logger;
            _baseUri = _configuration["CatalogUrl"];
        }

        public async Task<IList<Models.Product>> GetProducts()
        {
            var uri = _baseUri + ApiUris.GetProduct;
            var json = await _httpClient.GetStringAsync(uri);
            IList<Product> result = JsonConvert.DeserializeObject<IList<Models.Product>>(json);
            return result;
        }

        public async Task<IList<Product>> BuscaProducts(string pesquisa)
        {
            return await GetAsync<List<Product>>(ApiUris.SearchProducts, pesquisa);
        }

        public async Task<Models.Product> GetProduct(string codigo)
        {
            return await GetAsync<Product>(ApiUris.GetProduct, codigo);
        }

        public override string Scope => "Catalog.API";
    }
}
