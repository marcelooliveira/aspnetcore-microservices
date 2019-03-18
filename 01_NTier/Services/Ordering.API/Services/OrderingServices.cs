using AutoMapper;
using Ordering.Models.DTOs;
using Ordering.Repositories;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ordering.Controllers
{
    public class OrderingServices
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public object JwtClaimTypes { get; private set; }

        public OrderingServices(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<Order> Post(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            return await orderRepository.CreateOrUpdate(order);
        }

        public async Task<List<OrderDTO>> Get(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            IList<Order> orders = await orderRepository.GetOrders(customerId);

            if (orders == null)
            {
                throw new ArgumentException(nameof(customerId));
            }

            return mapper.Map<List<OrderDTO>>(orders);
        }
    }
}
