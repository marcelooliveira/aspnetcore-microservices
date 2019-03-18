using Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
    }

    public class Item
    {
        public ProductViewModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
        public int Id { get; set; }
    }

    public class OrderViewModel
    {
        public List<Item> Items { get; set; }
        public int RegistrationId { get; set; }
        public RegistrationViewModel Registration { get; set; }
        public int Id { get; set; }
    }
}
