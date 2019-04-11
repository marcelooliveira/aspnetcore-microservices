using Basket.API.Repositories;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IOrderingService _orderingServices;

        public BasketAPIService(IBasketRepository repository
            , IConfiguration configuration
            , IOrderingService orderingServices)
        {
            _repository = repository;
            _configuration = configuration;
            _orderingServices = orderingServices;
        }

        public CustomerBasket Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(nameof(id));
            }

            var basket = _repository.GetBasket(id);
            if (basket == null)
            {
                return new CustomerBasket(id);
            }
            return basket;
        }

        public CustomerBasket Post(CustomerBasket input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return _repository.UpdateBasket(input);
        }

        public CustomerBasket AddItem(string customerId, BasketItem input)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return _repository.AddBasket(customerId, input);
        }

        public UpdateQuantityOutput UpdateItem(string customerId, UpdateQuantityInput input)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return _repository.UpdateBasket(customerId, input);
        }

        public void Delete(string id)
        {
            _repository.DeleteBasket(id);
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

            CustomerBasket basket = _repository.GetBasket(customerId);

            var order
                = new Order(basket.Items.Select(i => new OrderItem(i)).ToList() 
                    , customerId, input.Name, input.Email, input.Phone
                    , input.Address, input.AdditionalAddress, input.District
                    , input.City, input.State, input.ZipCode);

            var newOrder = await _orderingServices.CreateOrUpdateAsync(order);

            _repository.DeleteBasket(customerId);

            return newOrder.Id;
        }
    }
}
