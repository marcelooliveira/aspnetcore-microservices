using AutoMapper;
using Microsoft.Extensions.Configuration;
using Models.ViewModels;
using MVC;
using Ordering.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly ISessionHelper sessionHelper;
        private readonly IOrderingService orderingService;

        public OrderService(IMapper mapper,
            IConfiguration configuration
            , ISessionHelper sessionHelper
            , IOrderingService orderingService) 
        {
            this.mapper = mapper;
            this.configuration = configuration;
            this.sessionHelper = sessionHelper;
            this.orderingService = orderingService;
        }

        public async Task<List<OrderDTO>> GetAsync(string customerId)
        {
            var list = await orderingService.GetListAsync(customerId);
            return mapper.Map<List<OrderDTO>>(list);
        }
    }
}
