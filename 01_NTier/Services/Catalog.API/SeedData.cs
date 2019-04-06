using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API
{
    public class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider services)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await context.Database.MigrateAsync();
                await SaveProducts(context);
            }
        }

        private static async Task SaveProducts(ApplicationDbContext context)
        {
            var productDbSet = context.Set<Product>();
            var categoryDbSet = context.Set<Category>();

            var products = await GetProducts();

            foreach (var product in products)
            {
                var categoryDB =
                categoryDbSet
                    .Where(c => c.Name == product.category)
                    .SingleOrDefault();

                if (categoryDB == null)
                {
                    categoryDB = new Category(product.category);
                    await categoryDbSet.AddAsync(categoryDB);
                    await context.SaveChangesAsync();
                }

                string code = product.number.ToString("000");
                if (!productDbSet.Where(p => p.Code == code).Any())
                {
                    await productDbSet.AddAsync(
                        new Product(code, product.name, 52.90m, categoryDB));
                }
            }
            await context.SaveChangesAsync();
        }

        static async Task<List<ProductData>> GetProducts()
        {
            var json = await File.ReadAllTextAsync("products.json");
            return JsonConvert.DeserializeObject<List<ProductData>>(json);
        }
    }

    public class ProductData
    {
        public int number { get; set; }
        public string name { get; set; }
        public string category { get; set; }
    }
}