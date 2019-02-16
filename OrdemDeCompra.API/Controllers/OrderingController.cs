using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using CasaDoCodigo.Ordering.Models;
using CasaDoCodigo.Ordering.Models.DTOs;
using CasaDoCodigo.Ordering.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Models;

namespace CasaDoCodigo.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        private readonly IPedidoRepository pedidoRepository;
        private readonly IMapper mapper;

        public object JwtClaimTypes { get; private set; }

        public OrderingController(IPedidoRepository pedidoRepository, IMapper mapper)
        {
            this.pedidoRepository = pedidoRepository;
            this.mapper = mapper;
        }

        // POST api/ordemdecompra
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await pedidoRepository.CreateOrUpdate(pedido);
            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("{clienteId}")]
        public async Task<ActionResult> Get(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
            {
                throw new ArgumentNullException();
            }

            IList<Pedido> pedidos = await pedidoRepository.GetPedidos(clienteId);

            if (pedidos == null)
            {
                return NotFound(clienteId);
            }

            List<PedidoDTO> dto = mapper.Map<List<PedidoDTO>>(pedidos);
            return base.Ok(dto);
        }

        private string GetUserId()
        {
            var userIdClaim = 
                User
                .Claims
                .FirstOrDefault(x => new[] 
                    {
                        "sub", ClaimTypes.NameIdentifier
                    }.Contains(x.Type)
                && !string.IsNullOrWhiteSpace(x.Value));

            if (userIdClaim != null)
                return userIdClaim.Value;

            throw new InvalidUserDataException("Usuário desconhecido");
        }
    }
}
