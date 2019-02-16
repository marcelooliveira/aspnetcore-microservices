using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Basket.API.Model
{
    public class BasketCliente
    {
        public BasketCliente()
        {
        }

        public BasketCliente(string clienteId)
        {
            ClienteId = clienteId;
            Itens = new List<ItemBasket>();
        }

        public BasketCliente(BasketCliente basketCliente)
        {
            this.ClienteId = basketCliente.ClienteId;
            this.Itens = basketCliente.Itens;
        }

        public string ClienteId { get; set; }
        public List<ItemBasket> Itens { get; set; }
        public decimal Total => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    }
}