﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.API.Model;
using CasaDoCodigo.API.Model.ViewModels;
using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : BaseApiController
    {
        private readonly IPedidoRepository pedidoRepository;

        public PedidoController(ILogger<CarrinhoController> logger,
            IPedidoRepository pedidoRepository) : base(logger)
        {
            this.pedidoRepository = pedidoRepository;
        }

        /// <summary>
        /// Obtém um pedido.
        /// </summary>
        /// <param name="id">O id do pedido</param>
        /// <returns>Um pedido com o id solicitado</returns>
        /// <response code="404">Id do pedido não encontrado</response>
        [HttpGet("{id}", Name = "Get")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            Pedido pedido = await pedidoRepository.GetPedido();
            if (pedido == null)
            {
                return BadRequest($"Id do pedido não encontrado: {id}");
            }

            PedidoViewModel viewModel = new PedidoViewModel(pedido);
            return Ok(viewModel);
        }
    }
}
