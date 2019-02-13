using Catalogo.API.Data;
using Catalogo.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API
{
    internal class SeedData
    {
        internal static async Task EnsureSeedData(IServiceProvider services)
        {
            using (var scope = services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await CreateTables(context);

                await SaveLivros(context);
            }
        }

        private static async Task CreateTables(ApplicationDbContext context)
        {
            await CreateTableCategoria(context);
            await CreateTableProduto(context);
        }

        private static async Task CreateTableCategoria(ApplicationDbContext context)
        {
            var sql
                = @"CREATE TABLE IF NOT EXISTS 
                        'Categoria' (
                            'Id'        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            'Nome'      TEXT NOT NULL
                        );";

            await ExecuteSqlCommandAsync(context, sql);
        }

        private static async Task CreateTableProduto(ApplicationDbContext context)
        {
            var sql
                = @"CREATE TABLE IF NOT EXISTS 
                        'Produto' (
                            'Id'        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        'Codigo'	TEXT NOT NULL,
                            'Nome'      TEXT NOT NULL,
	                        'Preco'     NUMERIC NOT NULL,
                            'CategoriaId' INTEGER NOT NULL,
                                FOREIGN KEY(CategoriaId) REFERENCES Categoria(Id)
                        );";

            await ExecuteSqlCommandAsync(context, sql);
        }

        private static async Task ExecuteSqlCommandAsync(ApplicationDbContext context, string createTableSql)
        {
            await context.Database.ExecuteSqlCommandAsync(createTableSql);
        }

        private static async Task SaveLivros(ApplicationDbContext context)
        {
            var produtoDbSet = context.Set<Produto>();
            var categoriaDbSet = context.Set<Categoria>();

            var products = await GetProducts();

            foreach (var product in products)
            {
                var categoriaDB =
                categoriaDbSet
                    .Where(c => c.Nome == product.category)
                    .SingleOrDefault();

                if (categoriaDB == null)
                {
                    categoriaDB = new Categoria(product.category);
                    await categoriaDbSet.AddAsync(categoriaDB);
                    await context.SaveChangesAsync();
                }

                string code = product.number.ToString("000");
                if (!produtoDbSet.Where(p => p.Codigo == code).Any())
                {
                    await produtoDbSet.AddAsync(new Produto(code, product.name, 52.90m, categoriaDB));
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