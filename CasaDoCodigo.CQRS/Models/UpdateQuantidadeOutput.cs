﻿using CasaDoCodigo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models
{
    public class UpdateQuantidadeOutput
    {
        public UpdateQuantidadeOutput(ItemBasket itemPedido, BasketCliente basketCliente)
        {
            ItemPedido = itemPedido;
            BasketCliente = basketCliente;
        }

        public ItemBasket ItemPedido { get; }
        public BasketCliente BasketCliente { get; }
    }
}
