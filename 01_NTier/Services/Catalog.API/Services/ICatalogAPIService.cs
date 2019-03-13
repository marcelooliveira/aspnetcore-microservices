using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Services
{
    public interface ICatalogAPIService
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(string code);
        Task<IEnumerable<Product>> SearchProducts(string search);
    }
}