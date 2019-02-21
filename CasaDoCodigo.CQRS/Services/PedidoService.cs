using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CasaDoCodigo;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.Extensions.Configuration;

namespace CasaDoCodigo.Services
{
    public class OrderService : BaseHttpService, IOrderService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ISessionHelper sessionHelper;

        public OrderService(IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper) 
            : base(configuration, httpClient, sessionHelper)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.sessionHelper = sessionHelper;
            _baseUri = _configuration["OrderingUrl"];
        }

        class Uris
        {
            public static string GetOrders => "api/ordemdecompra";
        }

        public async Task<List<OrderDTO>> GetAsync(string customerId)
        {
            return await GetAuthenticatedAsync<List<OrderDTO>>(Uris.GetOrders, customerId);
        }

        public override string Scope => "Ordering.API";
    }
}
