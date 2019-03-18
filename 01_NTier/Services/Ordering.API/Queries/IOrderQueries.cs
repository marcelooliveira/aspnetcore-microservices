using Ordering.Models;
using Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Queries
{
    public interface IOrderQueries
    {
        IList<Order> GetOrders(string customerId);
    }
}
