using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.Ordering.Commands
{
    public class CreateOrderCommand
        : IRequest<bool>
    {
        public CreateOrderCommand()
        {

        }

        public CreateOrderCommand(List<CreateOrderCommandItem> items, string customerId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            Items = items;
            CustomerId = customerId;
            CustomerName = clienteNome;
            CustomerEmail = clienteEmail;
            CustomerPhone = clienteTelefone;
            CustomerAddress = clienteEndereco;
            CustomerAdditionalCustomer = clienteComplemento;
            CustomerDistrict = clienteBairro;
            CustomerCity = clienteMunicipio;
            CustomerState = clienteUF;
            CustomerZipCode = clienteCEP;
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
        public int ProductQuantity { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public decimal Subtotal => ProductQuantity * ProductUnitPrice;

        public CreateOrderCommandItem()
        {

        }

        public CreateOrderCommandItem(string productCodigo, string productNome, int productQuantidade, decimal productPrecoUnitario)
        {
            ProductCode = productCodigo;
            ProductName = productNome;
            ProductQuantity = productQuantidade;
            ProductUnitPrice = productPrecoUnitario;
        }

        public void AtualizaQuantidade(int productQuantidade)
        {
            ProductQuantity = productQuantidade;
        }
    }
}
