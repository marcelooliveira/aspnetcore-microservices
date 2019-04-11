using AutoMapper;
using Microsoft.Extensions.Configuration;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderingService(IOrderRepository orderRepository
            , IMapper mapper
            , IConfiguration configuration)
        {
            this._orderRepository = orderRepository;
            this._mapper = mapper;
            this._configuration = configuration;
        }

        public async Task<Order> CreateOrUpdateAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            return await _orderRepository.CreateOrUpdate(order);
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
