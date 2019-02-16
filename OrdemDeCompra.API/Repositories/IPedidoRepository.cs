using CasaDoCodigo.Ordering.Models;
using CasaDoCodigo.Ordering.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Ordering.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> CreateOrUpdate(Pedido pedido);
        Task<IList<Pedido>> GetPedidos(string clienteId);
    }
}
