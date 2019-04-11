using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private static Dictionary<string, CustomerBasket> _database
            = new Dictionary<string, CustomerBasket>();

        public bool DeleteBasket(string id)
        {
            return _database.Remove(id);
        }

        public CustomerBasket GetBasket(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException();

            _database.TryGetValue(customerId, out CustomerBasket data);
            if (data == null)
            {
                return UpdateBasket(new CustomerBasket(customerId));
            }
            return data;
        }

        public CustomerBasket UpdateBasket(CustomerBasket basket)
        {
            if (_database.ContainsKey(basket.CustomerId))
            {
                _database[basket.CustomerId] = basket;
                return basket;
            }
            _database.Add(basket.CustomerId, basket);
            return basket;
        }

        public CustomerBasket AddBasket(string customerId, BasketItem item)
        {
            if (item == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(item.ProductId))
                throw new ArgumentException();

            if (item.Quantity <= 0)
                throw new ArgumentOutOfRangeException();

            var basket = GetBasket(customerId);
            BasketItem itemDB = basket.Items.Where(i => i.ProductId == item.ProductId).SingleOrDefault();
            if (itemDB == null)
            {
                itemDB = new BasketItem(item.Id, item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
                basket.Items.Add(item);
            }
            return UpdateBasket(basket);
        }

        public UpdateQuantityOutput UpdateBasket(string customerId, UpdateQuantityInput item)
        {
            if (item == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(item.ProductId))
                throw new ArgumentException();

            if (item.Quantity < 0)
                throw new ArgumentOutOfRangeException();

            var basket = GetBasket(customerId);
            BasketItem itemDB = basket.Items.Where(i => i.ProductId == item.ProductId).SingleOrDefault();
            itemDB.Quantity = item.Quantity;
            if (item.Quantity == 0)
            {
                basket.Items.Remove(itemDB);
            }
            CustomerBasket customerBasket = UpdateBasket(basket);
            return new UpdateQuantityOutput(itemDB, customerBasket);
        }
    }
}
