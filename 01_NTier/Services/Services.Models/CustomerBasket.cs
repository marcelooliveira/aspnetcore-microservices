using System.Collections.Generic;
using System.Linq;

namespace Services.Models
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
        }

        public CustomerBasket(string customerId)
        {
            CustomerId = customerId;
            Items = new List<BasketItem>();
        }

        public CustomerBasket(string customerId, List<BasketItem> items)
        {
            CustomerId = customerId;
            Items = items;
        }

        public CustomerBasket(CustomerBasket customerBasket)
        {
            this.CustomerId = customerBasket.CustomerId;
            this.Items = customerBasket.Items;
        }

        public string CustomerId { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}