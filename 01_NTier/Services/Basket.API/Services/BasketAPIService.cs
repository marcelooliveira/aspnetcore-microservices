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
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketAPIService> _logger;
        private readonly IConfiguration _configuration;
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

            return await _repository.UpdateBasketAsync(input);
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

            return await _repository.AddBasketAsync(customerId, input);
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

            return await _repository.UpdateBasketAsync(customerId, input);
        }

        public void Delete(string id)
        {
            _repository.DeleteBasketAsync(id);
        }

        public async Task<int> Checkout(string customerId, RegistrationViewModel input)
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

            var newOrder = await _orderingServices.CreateOrUpdateAsync(order);

            await _repository.DeleteBasketAsync(customerId);

            return newOrder.Id;
        }
    }
}
