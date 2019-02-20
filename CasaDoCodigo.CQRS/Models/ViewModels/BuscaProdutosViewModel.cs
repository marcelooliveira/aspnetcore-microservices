﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models.ViewModels
{
    public class BuscaProductsViewModel
    {
        public BuscaProductsViewModel(IList<Product> products, string search)
        {
            Products = products;
            Search = search;
        }

        public IList<Product> Products { get; }
        public string Search { get; set; }
    }
}
