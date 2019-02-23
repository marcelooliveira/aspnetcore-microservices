using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface IOrderService : IService
    {
        Task<List<OrderDTO>> GetAsync(string customerId);
    }
}
