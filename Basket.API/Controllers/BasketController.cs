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
    /// Provide basket functionalities for The Grocery Store
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
        /// Get the current user basket
        /// </summary>
        /// <param name="id">Current customer Id</param>
        /// <returns>Shopping basket</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
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
                return Ok(new CustomerBasket(id));
            }
            return Ok(basket);
        }

        //POST /value
        /// <summary>
        /// Saves the customer's shopping basket
        /// </summary>
        /// <param name="input">shopping basket data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Post([FromBody] CustomerBasket input)
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
        /// Add an item to the customer's shopping basket
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="input">New item to be added to customer's shopping basket</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]/{customerId}")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> AddItem(string customerId, [FromBody] BasketItem input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var basket = await _repository.AddBasketAsync(customerId, input);
                return Ok(basket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(customerId);
            }
        }

        /// <summary>
        /// Updates shopping basket item quantity
        /// </summary>
        /// <param name="customerId">CustomerId</param>
        /// <param name="input">Shopping basket item to be updated</param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]/{customerId}")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UpdateQuantityOutput>> UpdateItem(string customerId, [FromBody] UpdateQuantityInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var basket = await _repository.UpdateBasketAsync(customerId, input);
                return Ok(basket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(customerId);
            }

        }

        /// <summary>
        /// Removes the shopping basket item
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repository.DeleteBasketAsync(id);
        }

        [Route("[action]/{customerId}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> Checkout(string customerId, [FromBody] RegistryViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerBasket basket;
            try
            {
                basket = await _repository.GetBasketAsync(customerId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            var items = basket.Items.Select(i =>
                    new CheckoutEventItem(i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList();

            var checkoutEvent
                = new CheckoutEvent
                 (customerId, input.Name, input.Email, input.Phone
                    , input.Address, input.AdditionalAddress, input.District
                    , input.City, input.State, input.ZipCode
                    , Guid.NewGuid()
                    , items);

            //Once we complete it, it sends an integration event to API Ordering 
            //to convert the basket to order and continue with the order 
            //creation process
            await _bus.Publish(checkoutEvent);

            var registryEvent
                = new RegistryEvent
                 (customerId, input.Name, input.Email, input.Phone
                    , input.Address, input.AdditionalAddress, input.District
                    , input.City, input.State, input.ZipCode);

            await _bus.Publish(registryEvent);

            await _repository.DeleteBasketAsync(customerId);

            return Accepted(true);
        }
    }
}
