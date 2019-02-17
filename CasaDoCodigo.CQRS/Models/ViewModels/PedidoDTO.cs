using System;
using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Models.ViewModels
{
    public class PedidoDTO
    {
        public PedidoDTO()
        {

        }

        public PedidoDTO(List<ItemPedidoDTO> items, string id, string nome, string email, string telefone, string endereco, string complemento, string bairro, string municipio, string uF, string cEP)
        {
            Items = items;
            Id = id;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            Complemento = complemento;
            Bairro = bairro;
            Municipio = municipio;
            UF = uF;
            CEP = cEP;
        }

        public List<ItemPedidoDTO> Items { get; set; } = new List<ItemPedidoDTO>();
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Total => Items.Sum(i => i.Subtotal);
    }
}
