﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Mvc;
using CasaDoCodigo.API.Queries;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : BaseApiController
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IProdutoQueries produtoQueries;

        public ProdutoController(ILogger<ProdutoController> logger,
            IProdutoRepository produtoRepository, IProdutoQueries produtoQueries) : base(logger)
        {
            this.produtoRepository = produtoRepository;
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
            return Ok(await produtoQueries.GetProdutosAsync());
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos(string codigo = null)
        {
            return Ok(await produtoQueries.GetProdutoAsync(codigo));
        }
    }
}