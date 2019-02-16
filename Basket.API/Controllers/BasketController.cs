using Basket.API.Model;
using Basket.API.Services;
using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    /// <summary>
    /// Fornece funcionalidades do basket de compras da Casa do Código
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IBus _bus;
        
        public BasketController(IBasketRepository repository
            , IIdentityService identityService
            , IBus bus
            )
        {
            _repository = repository;
            _identityService = identityService;
            _bus = bus;
        }

        //GET /id
        /// <summary>
        /// Obtém o basket de compras
        /// </summary>
        /// <param name="id">Id do cliente do basket</param>
        /// <returns>O basket de compras</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BasketCliente), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

            var basket = await _repository.GetBasketAsync(id);
            if (basket == null)
            {
                return Ok(new BasketCliente(id));
            }
            return Ok(basket);
        }

        //POST /value
        /// <summary>
        /// Salva o basket de compras do cliente
        /// </summary>
        /// <param name="input">Dados do basket de compras</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BasketCliente), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Post([FromBody] BasketCliente input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(input);
            }

            try
            {
                var basket = await _repository.UpdateBasketAsync(input);
                return Ok(basket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Adiciona um item no basket de compras do cliente
        /// </summary>
        /// <param name="clienteId">Id do cliente</param>
        /// <param name="input">Novo item a inserir no basket de compras</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]/{clienteId}")]
        [ProducesResponseType(typeof(ItemBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCliente>> AddItem(string clienteId, [FromBody] ItemBasket input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var basket = await _repository.AddBasketAsync(clienteId, input);
                return Ok(basket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(clienteId);
            }
        }

        /// <summary>
        /// Atualiza a quantidade do item do basket de compras
        /// </summary>
        /// <param name="clienteId">Id do cliente</param>
        /// <param name="input">Item do basket de compras cuja quantidade será atualizada</param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]/{clienteId}")]
        [ProducesResponseType(typeof(ItemBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UpdateQuantidadeOutput>> UpdateItem(string clienteId, [FromBody] UpdateQuantidadeInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var basket = await _repository.UpdateBasketAsync(clienteId, input);
                return Ok(basket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(clienteId);
            }

        }

        /// <summary>
        /// Remove um item do basket de compras
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repository.DeleteBasketAsync(id);
        }

        [Route("[action]/{clienteId}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> Checkout(string clienteId, [FromBody] CadastroViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BasketCliente basket;
            try
            {
                basket = await _repository.GetBasketAsync(clienteId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            var itens = basket.Itens.Select(i =>
                    new CheckoutEventItem(i.Id, i.ProdutoId, i.ProdutoNome, i.PrecoUnitario, i.Quantidade)).ToList();

            var checkoutEvent
                = new CheckoutEvent
                 (clienteId, input.Nome, input.Email, input.Telefone
                    , input.Endereco, input.Complemento, input.Bairro
                    , input.Municipio, input.UF, input.CEP
                    , Guid.NewGuid()
                    , itens);

            // Assim que fazemos a finalização, envia um evento de integração para
            // API OrdemDeCompra converter o basket em pedido e continuar com
            // processo de criação de pedido
            await _bus.Publish(checkoutEvent);

            var cadastroEvent
                = new CadastroEvent
                 (clienteId, input.Nome, input.Email, input.Telefone
                    , input.Endereco, input.Complemento, input.Bairro
                    , input.Municipio, input.UF, input.CEP);

            await _bus.Publish(cadastroEvent);

            await _repository.DeleteBasketAsync(clienteId);

            return Accepted(true);
        }
    }
}
