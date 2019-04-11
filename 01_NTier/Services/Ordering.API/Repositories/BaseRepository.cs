using Microsoft.EntityFrameworkCore;
using Ordering.API;
using Services.Models;

namespace Ordering.Repositories
{
    public abstract class BaseRepository<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext contexto;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(ApplicationDbContext contexto)
        {
            this.contexto = contexto;
            dbSet = contexto.Set<T>();
        }
    }
}
