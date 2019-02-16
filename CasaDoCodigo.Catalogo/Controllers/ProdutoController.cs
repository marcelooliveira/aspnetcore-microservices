﻿using Catalog.API.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IProdutoQueries produtoQueries;

        public ProdutoController(ILogger<ProdutoController> logger,
            IProdutoQueries produtoQueries)
        {
            this.logger = logger;
            this.produtoQueries = produtoQueries;
        }

        /// <summary>
        /// Obtém a lista completa de produtos do catálogo.
        /// </summary>
        /// <returns>
        /// A lista completa de produtos do catálogo
        /// </returns>
        /// <response code="401">Não autorizado</response> 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            IEnumerable<Produto> produtos = await produtoQueries.GetProdutosAsync();
            return Ok(produtos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos(string codigo = null)
        {
            Produto value = await produtoQueries.GetProdutoAsync(codigo);
            if (value == null)
                return new NotFoundResult();
            return base.Ok(value);
        }
    }
}