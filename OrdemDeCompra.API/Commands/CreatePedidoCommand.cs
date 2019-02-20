using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.Ordering.Commands
{
    public class CreatePedidoCommand
        : IRequest<bool>
    {
        public CreatePedidoCommand()
        {

        }

        public CreatePedidoCommand(List<CreatePedidoCommandItem> items, string customerId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            Items = items;
            CustomerId = customerId;
            ClienteNome = clienteNome;
            ClienteEmail = clienteEmail;
            ClienteTelefone = clienteTelefone;
            ClienteEndereco = clienteEndereco;
            ClienteComplemento = clienteComplemento;
            ClienteBairro = clienteBairro;
            ClienteMunicipio = clienteMunicipio;
            ClienteUF = clienteUF;
            ClienteCEP = clienteCEP;
        }

        public List<CreatePedidoCommandItem> Items { get; private set; } = new List<CreatePedidoCommandItem>();
        public string CustomerId { get; set; } = "";
        public string ClienteNome { get; set; } = "";
        public string ClienteEmail { get; set; } = "";
        public string ClienteTelefone { get; set; } = "";
        public string ClienteEndereco { get; set; } = "";
        public string ClienteComplemento { get; set; } = "";
        public string ClienteBairro { get; set; } = "";
        public string ClienteMunicipio { get; set; } = "";
        public string ClienteUF { get; set; } = "";
        public string ClienteCEP { get; set; } = "";
    }

    public class CreatePedidoCommandItem
    {
        public CreatePedidoCommand Pedido { get; set; }
        public string ProductCodigo { get; set; }
        public string ProductNome { get; set; }
        public int ProductQuantidade { get; set; }
        public decimal ProductPrecoUnitario { get; set; }
        public decimal Subtotal => ProductQuantidade * ProductPrecoUnitario;

        public CreatePedidoCommandItem()
        {

        }

        public CreatePedidoCommandItem(string productCodigo, string productNome, int productQuantidade, decimal productPrecoUnitario)
        {
            ProductCodigo = productCodigo;
            ProductNome = productNome;
            ProductQuantidade = productQuantidade;
            ProductPrecoUnitario = productPrecoUnitario;
        }

        public void AtualizaQuantidade(int productQuantidade)
        {
            ProductQuantidade = productQuantidade;
        }
    }
}
