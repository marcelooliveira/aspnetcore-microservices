using AutoMapper;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IMapper _mapper;
        private readonly 
            Catalog.API.Services.ICatalogAPIService _apiService;

        public string Scope => "Catalog.API";

        public CatalogService(IMapper mapper,
            Catalog.API.Services.ICatalogAPIService apiService)
        {
            _mapper = mapper;
            _apiService = apiService;
        }

        public async Task<IList<Product>> GetProducts()
        {
            var products = await _apiService.GetProducts();
            return _mapper.Map<IList<Product>>(products);
        }

        public async Task<IList<Product>> SearchProducts(string search)
        {
            var products = await _apiService.SearchProducts(search);
            return _mapper.Map<IList<Product>>(products);
        }

        public async Task<Product> GetProduct(string code)
        {
            var product = await _apiService.GetProduct(code);
            return _mapper.Map<Product>(product);
        }
    }
}
