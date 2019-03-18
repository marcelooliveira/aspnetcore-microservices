﻿using Messages.IntegrationEvents.Events;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ordering.Services;
using Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Services
{
    public class BasketAPIService : IBasketAPIService
    {
        private EventId EventId_Checkout = new EventId(1001, "Checkout");
        private EventId EventId_Registry = new EventId(1002, "Registry");
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketAPIService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HubConnection _connection;
        private readonly IOrderingService _orderingServices;

        public BasketAPIService(IBasketRepository repository
            , ILogger<BasketAPIService> logger
            , IConfiguration configuration
            , IOrderingService orderingServices)
        {
            _repository = repository;
            _logger = logger;
            _configuration = configuration;
            _orderingServices = orderingServices;

            string userCounterDataHubUrl = $"{_configuration["SignalRServerUrl"]}usercounterdatahub";

            this._connection = new HubConnectionBuilder()
                .WithUrl(userCounterDataHubUrl, HttpTransportType.WebSockets)
                .Build();
            this._connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this._connection.StartAsync();
            };

            this._connection.StartAsync().GetAwaiter();
        }

        public async Task<CustomerBasket> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(nameof(id));
            }

            var basket = await _repository.GetBasketAsync(id);
            if (basket == null)
            {
                return new CustomerBasket(id);
            }
            return basket;
        }

        public async Task<CustomerBasket> Post(CustomerBasket input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var basket = await _repository.UpdateBasketAsync(input);

            await this._connection
                .InvokeAsync("UpdateUserBasketCount", $"{input.CustomerId}", basket.Items.Count);

            return basket;
        }

        public async Task<CustomerBasket> AddItem(string customerId, BasketItem input)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var basket = await _repository.AddBasketAsync(customerId, input);

            await this._connection
                .InvokeAsync("UpdateUserBasketCount", $"{customerId}", basket.Items.Count);

            return basket;

        }

        public async Task<UpdateQuantityOutput> UpdateItem(string customerId, UpdateQuantityInput input)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var output = await _repository.UpdateBasketAsync(customerId, input);

            await this._connection
                .InvokeAsync("UpdateUserBasketCount", $"{customerId}", output.CustomerBasket.Items.Count);

            return output;
        }

        public void Delete(string id)
        {
            _repository.DeleteBasketAsync(id);
        }

        public async Task<bool> Checkout(string customerId, RegistrationViewModel input)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            CustomerBasket basket = await _repository.GetBasketAsync(customerId);

            var order
                = new Order(basket.Items.Select(i => new OrderItem(i)).ToList() 
                    , customerId, input.Name, input.Email, input.Phone
                    , input.Address, input.AdditionalAddress, input.District
                    , input.City, input.State, input.ZipCode);

            await _orderingServices.CreateOrUpdateAsync(order);

            await _repository.DeleteBasketAsync(customerId);

            await this._connection
                .InvokeAsync("UpdateUserBasketCount", $"{customerId}", 0);

            return true;
        }
    }
}
