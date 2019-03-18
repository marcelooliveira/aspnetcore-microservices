﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ordering.Commands
{
    public class CreateOrderCommand
        : IRequest<bool>
    {
        public CreateOrderCommand()
        {

        }

        public CreateOrderCommand(List<CreateOrderCommandItem> items, string customerId, string customerName, string customerEmail, string customerPhone, string customerAddress, string customerAdditionalCustomer, string customerDistrict, string customerCity, string customerState, string customerZipCode)
        {
            Items = items;
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            CustomerAddress = customerAddress;
            CustomerAdditionalCustomer = customerAdditionalCustomer;
            CustomerDistrict = customerDistrict;
            CustomerCity = customerCity;
            CustomerState = customerState;
            CustomerZipCode = customerZipCode;
        }

        public List<CreateOrderCommandItem> Items { get; private set; } = new List<CreateOrderCommandItem>();
        public string CustomerId { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerAdditionalCustomer { get; set; } = "";
        public string CustomerDistrict { get; set; } = "";
        public string CustomerCity { get; set; } = "";
        public string CustomerState { get; set; } = "";
        public string CustomerZipCode { get; set; } = "";
    }

    public class CreateOrderCommandItem
    {
        public CreateOrderCommand Order { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;

        public CreateOrderCommandItem()
        {

        }

        public CreateOrderCommandItem(string productCodigo, string productNome, int quantity, decimal unitPrice)
        {
            ProductCode = productCodigo;
            ProductName = productNome;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void AtualizaQuantidade(int quantity)
        {
            Quantity = quantity;
        }
    }
}
