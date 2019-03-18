﻿using AutoMapper;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ordering.Models.DTOs;
using Ordering.Repositories;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.Services
{
    public class OrderingService : IOrderingService
    {
        private EventId EventId_CreateOrder = new EventId(1003, "Checkout");
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HubConnection _connection;

        public OrderingService(
            ILogger<OrderingService> logger
            , IOrderRepository orderRepository
            , IMapper mapper
            , IConfiguration configuration)
        {
            this._logger = logger;
            this._orderRepository = orderRepository;
            this._mapper = mapper;
            this._configuration = configuration;

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

        public async Task<Order> CreateOrUpdateAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            Order newOrder = null;
            try
            {
                newOrder = await _orderRepository.CreateOrUpdate(order);

                string notificationText = $"New order placed successfully: {newOrder.Id}";

                _logger.LogInformation(eventId: EventId_CreateOrder, message: "New order has been placed successfully: {Order}", newOrder);

                await this._connection.InvokeAsync("SendUserNotification",
                    $"{newOrder.CustomerId}", notificationText);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
            return newOrder;
        }

        public async Task<List<OrderDTO>> GetListAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            IList<Order> orders = await _orderRepository.GetOrders(customerId);

            if (orders == null)
            {
                throw new ArgumentException(nameof(customerId));
            }

            return _mapper.Map<List<OrderDTO>>(orders);
        }
    }
}