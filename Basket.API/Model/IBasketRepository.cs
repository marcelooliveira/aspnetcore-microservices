using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basket.API.Model
{
    public interface IBasketRepository
    {
        Task<BasketCliente> GetBasketAsync(string clienteId);
        IEnumerable<string> GetUsuarios();
        Task<BasketCliente> UpdateBasketAsync(BasketCliente basket);
        Task<BasketCliente> AddBasketAsync(string clienteId, ItemBasket item);
        Task<UpdateQuantidadeOutput> UpdateBasketAsync(string clienteId, UpdateQuantidadeInput item);
        Task<bool> DeleteBasketAsync(string id);
    }
}