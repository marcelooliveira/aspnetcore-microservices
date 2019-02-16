using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface ICatalogService : IService
    {
        Task<IList<Produto>> GetProdutos();
        Task<IList<Produto>> BuscaProdutos(string pesquisa);
        Task<Produto> GetProduto(string codigo);
    }
}
