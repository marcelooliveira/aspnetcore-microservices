using Models;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ICatalogService : IService
    {
        Task<IList<Product>> GetProducts();
        Task<Product> GetProduct(string code);
        Task<IList<Product>> SearchProducts(string search);
    }
}
