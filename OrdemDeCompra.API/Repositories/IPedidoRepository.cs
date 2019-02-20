using CasaDoCodigo.Ordering.Models;
using CasaDoCodigo.Ordering.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Ordering.Repositories
{
    public interface IPedidoRepository
    {
        Task<Order> CreateOrUpdate(Order pedido);
        Task<IList<Order>> GetPedidos(string customerId);
    }
}
