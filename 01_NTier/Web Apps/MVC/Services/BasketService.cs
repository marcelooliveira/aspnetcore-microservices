using AutoMapper;
using MVC.Model.UserData;
using Services.Models;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class BasketService : IBasketService
    {
        private readonly IMapper _mapper;
        private readonly
            Basket.API.Services.IBasketAPIService _apiService;
        private readonly IUserRedisRepository _userRedisRepository;


        public string Scope => "Basket.API";

        public BasketService(IMapper mapper,
            Basket.API.Services.IBasketAPIService apiService,
            IUserRedisRepository userRedisRepository)
        {
            _mapper = mapper;
            _apiService = apiService;
            _userRedisRepository = userRedisRepository;
        }

        public CustomerBasket GetBasket(string userId)
        {
            var basket = _apiService.Get(userId);
            return _mapper.Map<CustomerBasket>(basket);
        }

        public CustomerBasket AddItem(string customerId, BasketItem input)
        {
            var item = _mapper.Map<BasketItem>(input);
            var result = _apiService.AddItem(customerId, item);
            return _mapper.Map<CustomerBasket>(result);
        }

        public UpdateQuantityOutput UpdateItem(string customerId, UpdateQuantityInput input)
        {
            var updateQuantityInput = _mapper.Map<UpdateQuantityInput>(input);
            var result = _apiService.UpdateItem(customerId, updateQuantityInput);
            return _mapper.Map<UpdateQuantityOutput>(result);
        }

        public async Task<bool> CheckoutAsync(string customerId, RegistrationViewModel viewModel)
        {
            var input = _mapper.Map<RegistrationViewModel>(viewModel);
            var orderId = await _apiService.Checkout(customerId, input);
            string message = string.Format("New order placed successfully: {0}", orderId);
            var userNotification = new UserNotification(customerId, message, DateTime.Now, null);
            _userRedisRepository.AddUserNotification(customerId, userNotification);
            return true;
        }
    }
}
