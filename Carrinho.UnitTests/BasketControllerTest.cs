using Basket.API.Controllers;
using Basket.API.Model;
using CasaDoCodigo.Controllers;
using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MVC.Models;
using Rebus.Bus;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using IBasketIdentityService = Basket.API.Services.IIdentityService;

namespace Basket.API.Tests
{
    public class BasketControllerTest
    {
        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IBasketIdentityService> _identityServiceMock;
        private readonly Mock<IBus> _serviceBusMock;

        public BasketControllerTest()
        {
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _identityServiceMock = new Mock<IBasketIdentityService>();
            _serviceBusMock = new Mock<IBus>();
        }

        #region Get

        [Fact]
        public async Task Get_basket_cliente_sucesso()
        {
            //Arrange
            var fakeCustomerId = "1";
            var basketFake = GetCustomerBasketFake(fakeCustomerId);

            _basketRepositoryMock
                .Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(basketFake))
                .Verifiable();
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await basketController.Get(fakeCustomerId) as OkObjectResult;

            //Assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            CasaDoCodigo.Models.ViewModels.CustomerBasket customerBasket = Assert.IsAssignableFrom<CasaDoCodigo.Models.ViewModels.CustomerBasket>(okObjectResult.Value);
            Assert.Equal(fakeCustomerId, customerBasket.CustomerId);
            Assert.Equal(basketFake.Items[0].ProdutoId, customerBasket.Items[0].ProductId);
            Assert.Equal(basketFake.Items[1].ProdutoId, customerBasket.Items[1].ProductId);
            Assert.Equal(basketFake.Items[2].ProdutoId, customerBasket.Items[2].ProductId);
            _basketRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task Get_basket_cliente_no_client()
        {
            //arrange
            var controller =
                new BasketController(_basketRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object);

            //act
            IActionResult actionResult = await controller.Get(null);

            //assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.IsType<SerializableError>(badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Get_basket_cliente_basket_not_found()
        {
            //arrange
            string customerId = "123";
            CasaDoCodigo.Models.ViewModels.CustomerBasket basketFake = GetCustomerBasketFake(customerId);
            _basketRepositoryMock
                .Setup(r => r.GetBasketAsync(customerId))
                .ReturnsAsync((CasaDoCodigo.Models.ViewModels.CustomerBasket)null)
                .Verifiable();

            var controller =
                new BasketController(_basketRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object);

            //act
            IActionResult actionResult = await controller.Get(customerId);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            CasaDoCodigo.Models.ViewModels.CustomerBasket customerBasket = Assert.IsAssignableFrom<CasaDoCodigo.Models.ViewModels.CustomerBasket>(okObjectResult.Value);
            Assert.Equal(customerId, customerBasket.CustomerId);
            _basketRepositoryMock.Verify();
        }
        #endregion

        #region Post
        [Fact]
        public async Task Post_basket_cliente_sucesso()
        {
            //Arrange
            var fakeCustomerId = "1";
            var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

            _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CasaDoCodigo.Models.ViewModels.CustomerBasket>()))
                .Returns(Task.FromResult((CasaDoCodigo.Models.ViewModels.CustomerBasket)fakeCustomerBasket))
                .Verifiable();
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null))
                .Verifiable();

            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await basketController.Post(fakeCustomerBasket) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CasaDoCodigo.Models.ViewModels.CustomerBasket)actionResult.Value).CustomerId, fakeCustomerId);

            _basketRepositoryMock.Verify();
        }

        [Fact]
        public async Task Post_basket_cliente_not_found()
        {
            //Arrange
            var fakeCustomerId = "1";
            var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

            _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CasaDoCodigo.Models.ViewModels.CustomerBasket>()))
                .ThrowsAsync(new KeyNotFoundException())
                .Verifiable();
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await basketController.Post(fakeCustomerBasket);

            //Assert
            NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
            _basketRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task Post_basket_cliente_invalid_model()
        {
            //Arrange
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            controller.ModelState.AddModelError("CustomerId", "Required");

            //Act
            var actionResult = await controller.Post(new CasaDoCodigo.Models.ViewModels.CustomerBasket());

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        }
        
        #endregion

        #region Checkout
        [Fact]
        public async Task Fazer_Checkout_Sem_Cesta_Deve_Retornar_BadRequest()
        {
            //Arrange
            var fakeCustomerId = "2";

            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            CasaDoCodigo.Models.RegistryViewModel input = new CasaDoCodigo.Models.RegistryViewModel();
            basketController.ModelState.AddModelError("Email", "Required");

            //Act
            ActionResult<bool> actionResult = await basketController.Checkout(fakeCustomerId, input);

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Fazer_Checkout_Basket_Not_Found()
        {
            //Arrange
            var fakeCustomerId = "123";
            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .ThrowsAsync(new KeyNotFoundException());
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            CasaDoCodigo.Models.RegistryViewModel input = new CasaDoCodigo.Models.RegistryViewModel();

            //Act
            ActionResult<bool> actionResult = await basketController.Checkout(fakeCustomerId, input);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }


        [Fact]
        public async Task Fazer_Checkout_Com_Basket_Deveria_Publicar_CheckoutEvent()
        {
            //arrange
            var fakeCustomerId = "1";
            var fakeCustomerBasket = GetCustomerBasketFake(fakeCustomerId);

            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                 .Returns(Task.FromResult(fakeCustomerBasket))
                .Verifiable();

            //_identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId)
            //    .Verifiable();

            var basketController = new BasketController(
                _basketRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

            basketController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[] { new Claim("unique_name", "testuser") }))
                }
            };

            //Act
            ActionResult<bool> actionResult = await basketController.Checkout(fakeCustomerId, new CasaDoCodigo.Models.RegistryViewModel());

            //assert
            _serviceBusMock.Verify(mock => mock.Publish(It.IsAny<CheckoutEvent>(), null), Times.Once);
            Assert.NotNull(actionResult);
            _basketRepositoryMock.Verify();
            //_identityServiceMock.Verify();
        }

        #endregion

        #region AddItem
        [Fact]
        public async Task AddItem_success()
        {
            //arrange
            var customerId = "123";
            var basket = GetCustomerBasketFake(customerId);
            CasaDoCodigo.Models.ViewModels.BasketItem input = new CasaDoCodigo.Models.ViewModels.BasketItem("004", "004", "produto 004", 45.67m, 4);
            var items = basket.Items;
            items.Add(input);
            _basketRepositoryMock
                .Setup(c => c.AddBasketAsync(customerId, It.IsAny<CasaDoCodigo.Models.ViewModels.BasketItem>()))
                .ReturnsAsync(new CasaDoCodigo.Models.ViewModels.CustomerBasket
                {
                    CustomerId = customerId,
                    Items = items
                })
                .Verifiable();

            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<CasaDoCodigo.Models.ViewModels.CustomerBasket> actionResult = await controller.AddItem(customerId, input);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            CasaDoCodigo.Models.ViewModels.CustomerBasket customerBasket = Assert.IsAssignableFrom<CasaDoCodigo.Models.ViewModels.CustomerBasket>(okObjectResult.Value);
            Assert.Equal(4, customerBasket.Items.Count());
            _basketRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task AddItem_basket_notfound()
        {
            //arrange
            var customerId = "123";
            CasaDoCodigo.Models.ViewModels.BasketItem input = new CasaDoCodigo.Models.ViewModels.BasketItem("004", "004", "produto 004", 45.67m, 4);
            _basketRepositoryMock
                .Setup(c => c.AddBasketAsync(customerId, It.IsAny<CasaDoCodigo.Models.ViewModels.BasketItem>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<CasaDoCodigo.Models.ViewModels.CustomerBasket> actionResult = await controller.AddItem(customerId, input);

            //assert
            NotFoundObjectResult notFoundObjectResult =  Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(customerId, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task AddItem_basket_invalid_model()
        {
            //arrange
            var customerId = "123";
            CasaDoCodigo.Models.ViewModels.BasketItem input = new CasaDoCodigo.Models.ViewModels.BasketItem("004", "004", "produto 004", 45.67m, 4);
            _basketRepositoryMock
                .Setup(c => c.AddBasketAsync(customerId, It.IsAny<CasaDoCodigo.Models.ViewModels.BasketItem>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            controller.ModelState.AddModelError("ProdutoId", "Required");

            //act
            ActionResult<CasaDoCodigo.Models.ViewModels.CustomerBasket> actionResult = await controller.AddItem(customerId, input);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        #endregion

        #region UpdateItem
        [Fact]
        public async Task UpdateItem_success()
        {
            //arrange
            var customerId = "123";
            var basket = GetCustomerBasketFake(customerId);
            CasaDoCodigo.Models.ViewModels.BasketItem input = new CasaDoCodigo.Models.ViewModels.BasketItem("004", "004", "produto 004", 45.67m, 4);
            var items = basket.Items;
            items.Add(input);
            _basketRepositoryMock
                .Setup(c => c.UpdateBasketAsync(customerId, It.IsAny<MVC.Models.UpdateQuantityInput>()))
                .ReturnsAsync(new CasaDoCodigo.Models.UpdateQuantityOutput(input,
                new CasaDoCodigo.Models.ViewModels.CustomerBasket
                {
                    CustomerId = customerId,
                    Items = items
                }))
                .Verifiable();

            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<CasaDoCodigo.Models.UpdateQuantityOutput> actionResult = await controller.UpdateItem(customerId, new MVC.Models.UpdateQuantityInput(input.ProductId, input.Quantity));

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            CasaDoCodigo.Models.UpdateQuantityOutput updateQuantidadeOutput = Assert.IsAssignableFrom<CasaDoCodigo.Models.UpdateQuantityOutput>(okObjectResult.Value);
            Assert.Equal(input.ProductId, updateQuantidadeOutput.BasketItem.ProductId);
            _basketRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task UpdateItem_basket_notfound()
        {
            //arrange
            var customerId = "123";
            CasaDoCodigo.Models.ViewModels.BasketItem input = new CasaDoCodigo.Models.ViewModels.BasketItem("004", "004", "produto 004", 45.67m, 4);
            _basketRepositoryMock
                .Setup(c => c.UpdateBasketAsync(customerId, It.IsAny<MVC.Models.UpdateQuantityInput>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<CasaDoCodigo.Models.UpdateQuantityOutput> actionResult = await controller.UpdateItem(customerId, new MVC.Models.UpdateQuantityInput(input.ProductId, input.Quantity));

            //assert
            NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(customerId, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task UpdateItem_basket_invalid_model()
        {
            //arrange
            var customerId = "123";
            CasaDoCodigo.Models.ViewModels.BasketItem input = new CasaDoCodigo.Models.ViewModels.BasketItem("004", "004", "produto 004", 45.67m, 4);
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            controller.ModelState.AddModelError("ProdutoId", "Required");

            //act
            ActionResult<CasaDoCodigo.Models.UpdateQuantityOutput> actionResult = await controller.UpdateItem(customerId, new MVC.Models.UpdateQuantityInput(input.ProductId, input.Quantity));

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        #endregion

        private CustomerBasket GetCustomerBasketFake(string fakeCustomerId)
        {
            return new CasaDoCodigo.Models.ViewModels.CustomerBasket(fakeCustomerId)
            {
                CustomerId = fakeCustomerId,
                Items = new List<CasaDoCodigo.Models.ViewModels.BasketItem>()
                {
                    new CasaDoCodigo.Models.ViewModels.BasketItem("001", "001", "produto 001", 12.34m, 1),
                    new CasaDoCodigo.Models.ViewModels.BasketItem("002", "002", "produto 002", 23.45m, 2),
                    new CasaDoCodigo.Models.ViewModels.BasketItem("003", "003", "produto 003", 34.56m, 3)
                }
            };
        }

    }
}
