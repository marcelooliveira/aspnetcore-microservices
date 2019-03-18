using System.Collections.Generic;
using System.Threading.Tasks;
using Ordering.Models.DTOs;
using Services.Models;

namespace Ordering.Services
{
    public interface IOrderingService
    {
        Task<Order> CreateOrUpdateAsync(Order order);
        Task<List<OrderDTO>> GetListAsync(string customerId);
    }
}