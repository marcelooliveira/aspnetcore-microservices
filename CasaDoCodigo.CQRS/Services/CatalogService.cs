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
            public static string GetProduto => "api/produto";
            public static string BuscaProdutos => "api/busca";
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

        public async Task<IList<Models.Produto>> GetProdutos()
        {
            var uri = _baseUri + ApiUris.GetProduto;
            var json = await _httpClient.GetStringAsync(uri);
            IList<Produto> result = JsonConvert.DeserializeObject<IList<Models.Produto>>(json);
            return result;
        }

        public async Task<IList<Produto>> BuscaProdutos(string pesquisa)
        {
            return await GetAsync<List<Produto>>(ApiUris.BuscaProdutos, pesquisa);
        }

        public async Task<Models.Produto> GetProduto(string codigo)
        {
            return await GetAsync<Produto>(ApiUris.GetProduto, codigo);
        }

        public override string Scope => "Catalog.API";
    }
}
