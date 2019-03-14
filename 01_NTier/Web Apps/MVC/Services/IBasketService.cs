using Models;
using Models.ViewModels;
using MVC.Models;
using System.Threading.Tasks;

namespace Services
{
    public interface IBasketService : IService
    {
        Task<CustomerBasket> GetBasket(string userId);
        Task<CustomerBasket> AddItem(string customerId, BasketItem input);
        Task<UpdateQuantityOutput> UpdateItem(string customerId, UpdateQuantityInput input);
        Task<bool> Checkout(string customerId, RegistrationViewModel viewModel);
    }
}
