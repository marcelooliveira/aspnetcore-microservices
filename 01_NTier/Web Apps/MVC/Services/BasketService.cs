using AutoMapper;
using Models;
using Models.ViewModels;
using Services.Models;
using System.Threading.Tasks;

namespace Services
{
    public class BasketService : IBasketService
    {
        private readonly IMapper _mapper;
        private readonly
            Basket.API.Services.IBasketAPIService _apiService;

        public string Scope => "Basket.API";

        public BasketService(IMapper mapper,
            Basket.API.Services.IBasketAPIService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }

        public async Task<CustomerBasket> GetBasket(string userId)
        {
            var basket = await _apiService.Get(userId);
            return _mapper.Map<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket> AddItem(string customerId, BasketItem input)
        {
            var item = _mapper.Map<BasketItem>(input);
            var result = await _apiService.AddItem(customerId, item);
            return _mapper.Map<CustomerBasket>(result);
        }

        public async Task<UpdateQuantityOutput> UpdateItem(string customerId, UpdateQuantityInput input)
        {
            var updateQuantityInput = _mapper.Map<UpdateQuantityInput>(input);
            var result = await _apiService.UpdateItem(customerId, updateQuantityInput);
            return _mapper.Map<UpdateQuantityOutput>(result);
        }

        public async Task<bool> Checkout(string customerId, RegistrationViewModel viewModel)
        {
            var input = _mapper.Map<RegistrationViewModel>(viewModel);
            return await _apiService.Checkout(customerId, input);
        }
    }
}
