using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models.ViewModels
{
    public class BasketCliente
    {
        public BasketCliente()
        {

        }

        public BasketCliente(string clienteId, List<ItemBasket> itens)
        {
            ClienteId = clienteId;
            Itens = itens;
        }

        public string ClienteId { get; set; }
        public List<ItemBasket> Itens { get; set; } = new List<ItemBasket>();
        public decimal Total => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    }

    public class ItemBasket
    {
        public ItemBasket()
        {

        }

        public ItemBasket(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade, string urlImagem)
        {   
            Id = id;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
            UrlImagem = urlImagem;
        }

        public string Id { get; set; }
        public string ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
        public string UrlImagem { get; set; }
    }
}
