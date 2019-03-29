using Services.Models;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        CustomerBasket GetBasket(string customerId);
        CustomerBasket UpdateBasket(CustomerBasket basket);
        CustomerBasket AddBasket(string customerId, BasketItem item);
        UpdateQuantityOutput UpdateBasket(string customerId, UpdateQuantityInput item);
        bool DeleteBasket(string id);
    }
}