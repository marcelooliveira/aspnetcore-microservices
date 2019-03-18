using System.Threading.Tasks;
using Basket.API.Model;
using Services.Models;

namespace Basket.API.Services
{
    public interface IBasketAPIService
    {
        Task<CustomerBasket> AddItem(string customerId, BasketItem input);
        Task<bool> Checkout(string customerId, RegistrationViewModel input);
        void Delete(string id);
        Task<CustomerBasket> Get(string id);
        Task<CustomerBasket> Post(CustomerBasket input);
        Task<UpdateQuantityOutput> UpdateItem(string customerId, UpdateQuantityInput input);
    }
}