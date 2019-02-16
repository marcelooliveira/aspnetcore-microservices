﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Queries
{
    public interface IProdutoQueries
    {
        Task<IEnumerable<Produto>> GetProdutosAsync(string pesquisa = null);
        Task<Produto> GetProdutoAsync(string codigo);
    }
}