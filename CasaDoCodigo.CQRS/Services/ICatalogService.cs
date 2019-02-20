using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface ICatalogService : IService
    {
        Task<IList<Product>> GetProducts();
        Task<IList<Product>> BuscaProducts(string pesquisa);
        Task<Product> GetProduct(string codigo);
    }
}
