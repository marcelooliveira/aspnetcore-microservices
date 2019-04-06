using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
                
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasKey(t => t.Id);

            modelBuilder.Entity<Product>().HasKey(t => t.Id);
            modelBuilder.Entity<Product>().HasOne(t => t.Category);
        }
    }
}




