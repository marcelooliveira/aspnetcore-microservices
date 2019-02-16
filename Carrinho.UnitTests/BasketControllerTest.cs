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
            var fakeClienteId = "1";
            var basketFake = GetBasketClienteFake(fakeClienteId);

            _basketRepositoryMock
                .Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(basketFake))
                .Verifiable();
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await basketController.Get(fakeClienteId) as OkObjectResult;

            //Assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            BasketCliente basketCliente = Assert.IsAssignableFrom<BasketCliente>(okObjectResult.Value);
            Assert.Equal(fakeClienteId, basketCliente.ClienteId);
            Assert.Equal(basketFake.Itens[0].ProdutoId, basketCliente.Itens[0].ProdutoId);
            Assert.Equal(basketFake.Itens[1].ProdutoId, basketCliente.Itens[1].ProdutoId);
            Assert.Equal(basketFake.Itens[2].ProdutoId, basketCliente.Itens[2].ProdutoId);
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
            string clienteId = "123";
            BasketCliente basketFake = GetBasketClienteFake(clienteId);
            _basketRepositoryMock
                .Setup(r => r.GetBasketAsync(clienteId))
                .ReturnsAsync((BasketCliente)null)
                .Verifiable();

            var controller =
                new BasketController(_basketRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object);

            //act
            IActionResult actionResult = await controller.Get(clienteId);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            BasketCliente basketCliente = Assert.IsAssignableFrom<BasketCliente>(okObjectResult.Value);
            Assert.Equal(clienteId, basketCliente.ClienteId);
            _basketRepositoryMock.Verify();
        }
        #endregion

        #region Post
        [Fact]
        public async Task Post_basket_cliente_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var fakeBasketCliente = GetBasketClienteFake(fakeClienteId);

            _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<BasketCliente>()))
                .Returns(Task.FromResult(fakeBasketCliente))
                .Verifiable();
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null))
                .Verifiable();

            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await basketController.Post(fakeBasketCliente) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((BasketCliente)actionResult.Value).ClienteId, fakeClienteId);

            _basketRepositoryMock.Verify();
        }

        [Fact]
        public async Task Post_basket_cliente_not_found()
        {
            //Arrange
            var fakeClienteId = "1";
            var fakeBasketCliente = GetBasketClienteFake(fakeClienteId);

            _basketRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<BasketCliente>()))
                .ThrowsAsync(new KeyNotFoundException())
                .Verifiable();
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await basketController.Post(fakeBasketCliente);

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
            controller.ModelState.AddModelError("ClienteId", "Required");

            //Act
            var actionResult = await controller.Post(new BasketCliente());

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        }
        
        #endregion

        #region Checkout
        [Fact]
        public async Task Fazer_Checkout_Sem_Cesta_Deve_Retornar_BadRequest()
        {
            //Arrange
            var fakeClienteId = "2";

            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            CadastroViewModel input = new CadastroViewModel();
            basketController.ModelState.AddModelError("Email", "Required");

            //Act
            ActionResult<bool> actionResult = await basketController.Checkout(fakeClienteId, input);

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Fazer_Checkout_Basket_Not_Found()
        {
            //Arrange
            var fakeClienteId = "123";
            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                .ThrowsAsync(new KeyNotFoundException());
            var basketController = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            CadastroViewModel input = new CadastroViewModel();

            //Act
            ActionResult<bool> actionResult = await basketController.Checkout(fakeClienteId, input);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }


        [Fact]
        public async Task Fazer_Checkout_Com_Basket_Deveria_Publicar_CheckoutEvent()
        {
            //arrange
            var fakeClienteId = "1";
            var fakeBasketCliente = GetBasketClienteFake(fakeClienteId);

            _basketRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
                 .Returns(Task.FromResult(fakeBasketCliente))
                .Verifiable();

            //_identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId)
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
            ActionResult<bool> actionResult = await basketController.Checkout(fakeClienteId, new CadastroViewModel());

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
            var clienteId = "123";
            var basket = GetBasketClienteFake(clienteId);
            ItemBasket input = new ItemBasket("004", "004", "produto 004", 45.67m, 4);
            var itens = basket.Itens;
            itens.Add(input);
            _basketRepositoryMock
                .Setup(c => c.AddBasketAsync(clienteId, It.IsAny<ItemBasket>()))
                .ReturnsAsync(new BasketCliente
                {
                    ClienteId = clienteId,
                    Itens = itens
                })
                .Verifiable();

            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<BasketCliente> actionResult = await controller.AddItem(clienteId, input);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            BasketCliente basketCliente = Assert.IsAssignableFrom<BasketCliente>(okObjectResult.Value);
            Assert.Equal(4, basketCliente.Itens.Count());
            _basketRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task AddItem_basket_notfound()
        {
            //arrange
            var clienteId = "123";
            ItemBasket input = new ItemBasket("004", "004", "produto 004", 45.67m, 4);
            _basketRepositoryMock
                .Setup(c => c.AddBasketAsync(clienteId, It.IsAny<ItemBasket>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<BasketCliente> actionResult = await controller.AddItem(clienteId, input);

            //assert
            NotFoundObjectResult notFoundObjectResult =  Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(clienteId, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task AddItem_basket_invalid_model()
        {
            //arrange
            var clienteId = "123";
            ItemBasket input = new ItemBasket("004", "004", "produto 004", 45.67m, 4);
            _basketRepositoryMock
                .Setup(c => c.AddBasketAsync(clienteId, It.IsAny<ItemBasket>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            controller.ModelState.AddModelError("ProdutoId", "Required");
            
            //act
            ActionResult<BasketCliente> actionResult = await controller.AddItem(clienteId, input);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        #endregion

        #region UpdateItem
        [Fact]
        public async Task UpdateItem_success()
        {
            //arrange
            var clienteId = "123";
            var basket = GetBasketClienteFake(clienteId);
            ItemBasket input = new ItemBasket("004", "004", "produto 004", 45.67m, 4);
            var itens = basket.Itens;
            itens.Add(input);
            _basketRepositoryMock
                .Setup(c => c.UpdateBasketAsync(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ReturnsAsync(new UpdateQuantidadeOutput(input,
                new BasketCliente
                {
                    ClienteId = clienteId,
                    Itens = itens
                }))
                .Verifiable();

            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<UpdateQuantidadeOutput> actionResult = await controller.UpdateItem(clienteId, new UpdateQuantidadeInput(input.ProdutoId, input.Quantidade));

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            UpdateQuantidadeOutput updateQuantidadeOutput = Assert.IsAssignableFrom<UpdateQuantidadeOutput>(okObjectResult.Value);
            Assert.Equal(input.ProdutoId, updateQuantidadeOutput.ItemPedido.ProdutoId);
            _basketRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task UpdateItem_basket_notfound()
        {
            //arrange
            var clienteId = "123";
            ItemBasket input = new ItemBasket("004", "004", "produto 004", 45.67m, 4);
            _basketRepositoryMock
                .Setup(c => c.UpdateBasketAsync(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            //act
            ActionResult<UpdateQuantidadeOutput> actionResult = await controller.UpdateItem(clienteId, new UpdateQuantidadeInput(input.ProdutoId, input.Quantidade));

            //assert
            NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(clienteId, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task UpdateItem_basket_invalid_model()
        {
            //arrange
            var clienteId = "123";
            ItemBasket input = new ItemBasket("004", "004", "produto 004", 45.67m, 4);
            var controller = new BasketController(
                _basketRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            controller.ModelState.AddModelError("ProdutoId", "Required");

            //act
            ActionResult<UpdateQuantidadeOutput> actionResult = await controller.UpdateItem(clienteId, new UpdateQuantidadeInput(input.ProdutoId, input.Quantidade));

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        #endregion

        private BasketCliente GetBasketClienteFake(string fakeClienteId)
        {
            return new BasketCliente(fakeClienteId)
            {
                ClienteId = fakeClienteId,
                Itens = new List<ItemBasket>()
                {
                    new ItemBasket("001", "001", "produto 001", 12.34m, 1),
                    new ItemBasket("002", "002", "produto 002", 23.45m, 2),
                    new ItemBasket("003", "003", "produto 003", 34.56m, 3)
                }
            };
        }

    }
}
