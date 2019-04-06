using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using model = Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Queries
{
    public class ProductQueries : IProductQueries
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;

        public ProductQueries(IConfiguration configuration,
            ApplicationDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string search = null)
        {
            var query =
                from p in context.Set<model.Product>()
                    .Include(p => p.Category)
                select new Product
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.Category.Id,
                    CategoryName = p.Category.Name
                };

            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }

            return
                query = query
                    .Where(p =>
                        p.Name.StartsWith(search)
                        || p.CategoryName.StartsWith(search));
        }

        public async Task<Product> GetProductAsync(string code)
        {
            return await 
                context.Set<model.Product>()
                    .Include(p => p.Category)
                .Where(p => p.Code == code)
                .Select(p =>
                    new Product
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        Price = p.Price,
                        CategoryId = p.Category.Id,
                        CategoryName = p.Category.Name
                    })
                .SingleOrDefaultAsync();
        }
    }
}
