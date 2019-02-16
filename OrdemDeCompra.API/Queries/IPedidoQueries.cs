using CasaDoCodigo.Ordering.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Queries
{
    public interface IPedidoQueries
    {
        IList<Pedido> GetPedidos(string clienteId);
    }
}
