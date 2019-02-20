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
        public async Task<IActionResult> Post([FromBody] Order pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await pedidoRepository.CreateOrUpdate(pedido);
            return Ok(resultado);
        }

        [Authorize]
        [HttpGet("{customerId}")]
        public async Task<ActionResult> Get(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException();
            }

            IList<Order> pedidos = await pedidoRepository.GetPedidos(customerId);

            if (pedidos == null)
            {
                return NotFound(customerId);
            }

            List<OrderDTO> dto = mapper.Map<List<OrderDTO>>(pedidos);
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
