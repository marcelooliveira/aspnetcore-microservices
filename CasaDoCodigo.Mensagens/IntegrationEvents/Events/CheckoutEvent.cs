using System;
using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Mensagens.Events
{
    public class CheckoutEvent : IntegrationEvent
    {
        public CheckoutEvent()
        {

        }

        public CheckoutEvent(
              string userId, string userName, string email, string fone
            , string endereco, string complemento, string bairro
            , string municipio, string uf, string cep
            , Guid requestId
            , IList<CheckoutEventItem> items)
        {
            UserId = userId;
            UserName = userName;
            Municipio = municipio;
            Email = email;
            Fone = fone;
            Endereco = endereco;
            Complemento = complemento;
            Bairro = bairro;
            UF = uf;
            Cep = cep;
            RequestId = requestId;
            Items = 
                items
                    .Select(i => 
                        new CheckoutEventItem(
                            i.Id, 
                            i.ProductId, 
                            i.ProductNome, 
                            i.PrecoUnitario, 
                            i.Quantidade)).ToList();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public int PedidoId { get; set; }
        public string Municipio { get; set; }
        public string Email { get; set; }
        public string Fone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public Guid RequestId { get; set; }
        public List<CheckoutEventItem> Items { get; } = new List<CheckoutEventItem>();
    }

    public class CheckoutEventItem
    {
        public CheckoutEventItem()
        {

        }

        public CheckoutEventItem(string id, string productId, string productNome, decimal precoUnitario, int quantidade)
        {
            Id = id;
            ProductId = productId;
            ProductNome = productNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string ImageURL { get; set; }
    }

}
