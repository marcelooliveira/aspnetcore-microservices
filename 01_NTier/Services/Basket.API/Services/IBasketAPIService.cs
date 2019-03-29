using Services.Models;
using System.Threading.Tasks;

namespace Basket.API.Services
{
    public interface IBasketAPIService
    {
        CustomerBasket AddItem(string customerId, BasketItem input);
        Task<int> Checkout(string customerId, RegistrationViewModel input);
        void Delete(string id);
        CustomerBasket Get(string id);
        CustomerBasket Post(CustomerBasket input);
        UpdateQuantityOutput UpdateItem(string customerId, UpdateQuantityInput input);
    }
}