using CasaDoCodigo.Infrastructure;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MVC.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public class BasketService : BaseHttpService, IBasketService
    {
        class BasketUris
        {
            public static string GetBasket => "api/basket";
            public static string AddItem => "api/basket/additem";
            public static string UpdateItem => "api/basket/updateitem";
            public static string Finalizar => "api/basket/checkout";
            //public static string GetBasket => "basket"; //ApiGateway
            //public static string PostItem => "basket"; //ApiGateway
        }

        private readonly HttpClient _apiClient;
        private readonly string _basketUrl;
        private readonly ILogger<BasketService> _logger;

        public BasketService(
            IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper
            , ILogger<BasketService> logger)
            : base(configuration, httpClient, sessionHelper)
        {
            _apiClient = httpClient;
            _logger = logger;
            _baseUri = _configuration["BasketUrl"];
        }

        public async Task<BasketCliente> GetBasket(string userId)
        {
            return await GetAsync<BasketCliente>(BasketUris.GetBasket, userId);
        }

        public async Task<BasketCliente> AddItem(string clienteId, ItemBasket input)
        {
            var uri = $"{BasketUris.AddItem}/{clienteId}";
            return await PostAsync<BasketCliente>(uri, input);
        }

        public async Task<UpdateQuantidadeOutput> UpdateItem(string clienteId, UpdateQuantidadeInput input)
        {
            var uri = $"{BasketUris.UpdateItem}/{clienteId}";
            return await PutAsync<UpdateQuantidadeOutput>(uri, input);
        }

        public async Task<BasketCliente> DefinirQuantidades(ApplicationUser applicationUser, Dictionary<string, int> quantidades)
        {
            var uri = UrlAPIs.Basket.UpdateItemBasket(_basketUrl);

            var atualizarBasket = new
            {
                ClienteId = applicationUser.Id,
                Atualizacao = quantidades.Select(kvp => new
                {
                    ItemBasketId = kvp.Key,
                    NovaQuantidade = kvp.Value
                }).ToArray()
            };

            var conteudoBasket = new StringContent(JsonConvert.SerializeObject(atualizarBasket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PutAsync(uri, conteudoBasket);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<BasketCliente>(jsonResponse);
        }

        public Task AtualizarBasket(BasketCliente basketCliente)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Checkout(string clienteId, CadastroViewModel viewModel)
        {
            var uri = $"{BasketUris.Finalizar}/{clienteId}";
            return await PostAsync<bool>(uri, viewModel);
        }

        public override string Scope => "Basket.API";
    }
}
