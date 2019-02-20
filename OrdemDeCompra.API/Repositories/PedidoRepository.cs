using CasaDoCodigo.Ordering;
using CasaDoCodigo.Ordering.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Ordering.Repositories
{
    public class PedidoRepository : BaseRepository<Order>, IPedidoRepository
    {
        public PedidoRepository(ApplicationContext contexto) : base(contexto)
        {
        }

        public async Task<Order> CreateOrUpdate(Order pedido)
        {
            if (pedido == null)
                throw new ArgumentNullException();

            if (pedido.Items.Count == 0)
                throw new NoItemsException();

            foreach (var item in pedido.Items)
            {
                if (
                    string.IsNullOrWhiteSpace(item.ProductCode)
                    || string.IsNullOrWhiteSpace(item.ProductName)
                    || item.ProductQuantity <= 0
                    || item.ProductUnitPrice <= 0
                    )
                {
                    throw new InvalidItemException();
                }
            }

            if (string.IsNullOrWhiteSpace(pedido.CustomerId)
                 || string.IsNullOrWhiteSpace(pedido.CustomerName)
                 || string.IsNullOrWhiteSpace(pedido.CustomerEmail)
                 || string.IsNullOrWhiteSpace(pedido.CustomerPhone)
                 || string.IsNullOrWhiteSpace(pedido.CustomerAddress)
                 || string.IsNullOrWhiteSpace(pedido.CustomerAdditionalAddress)
                 || string.IsNullOrWhiteSpace(pedido.CustomerDistrict)
                 || string.IsNullOrWhiteSpace(pedido.CustomerCity)
                 || string.IsNullOrWhiteSpace(pedido.CustomerState)
                 || string.IsNullOrWhiteSpace(pedido.CustomerZipCode)
                )
                throw new InvalidUserDataException();

            EntityEntry<Order> entityEntry;
            try
            {
                entityEntry = await dbSet.AddAsync(pedido);
                await contexto.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return entityEntry.Entity;
        }

        public async Task<IList<Order>> GetPedidos(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException();
            }
            return await 
                dbSet
                .Include(p => p.Items)
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }
    }


    [Serializable]
    public class NoItemsException : Exception
    {
        public NoItemsException() {}
        public NoItemsException(string message) : base(message) { }
        public NoItemsException(string message, Exception inner) : base(message, inner) { }
        protected NoItemsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InvalidItemException : Exception
    {
        public InvalidItemException() { }
        public InvalidItemException(string message) : base(message) { }
        public InvalidItemException(string message, Exception inner) : base(message, inner) { }
        protected InvalidItemException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InvalidUserDataException : Exception
    {
        public InvalidUserDataException() { }
        public InvalidUserDataException(string message) : base(message) { }
        public InvalidUserDataException(string message, Exception inner) : base(message, inner) { }
        protected InvalidUserDataException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
