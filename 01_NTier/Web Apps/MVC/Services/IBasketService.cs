using Services.Models;
using System.Threading.Tasks;

namespace Services
{
    public interface IBasketService : IService
    {
        CustomerBasket GetBasket(string userId);
        CustomerBasket AddItem(string customerId, BasketItem input);
        UpdateQuantityOutput UpdateItem(string customerId, UpdateQuantityInput input);
        Task<bool> CheckoutAsync(string customerId, RegistrationViewModel viewModel);
    }
}
