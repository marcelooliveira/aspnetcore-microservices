using Basket.API.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Basket.API.Tests
{
    public class RedisBasketRepositoryTest
    {
        private readonly Mock<ILogger<RedisBasketRepository>> loggerMock;
        private readonly Mock<IConnectionMultiplexer> redisMock;

        public RedisBasketRepositoryTest()
        {
            loggerMock = new Mock<ILogger<RedisBasketRepository>>();
            redisMock = new Mock<IConnectionMultiplexer>();
        }

        #region GetBasketAsync
        [Fact]
        public async Task GetBasketAsync_success()
        {
            //arrange
            var json = @"{
                  ""ClienteId"": ""123"",
                  ""Itens"": [{
                  ""Id"": ""001"",
                  ""ProdutoId"": ""001"",
                  ""ProdutoNome"": ""Produto 001"",
                  ""Quantidade"": 7,
                  ""PrecoUnitario"": 12.34}]
                }";

            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(json)
                .Verifiable();
            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();

            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            var basketCliente = await repository.GetBasketAsync(clienteId);

            //assert
            Assert.Equal(clienteId, basketCliente.ClienteId);
            Assert.Collection(basketCliente.Itens,
                item =>
                {
                    Assert.Equal("001", item.ProdutoId);
                    Assert.Equal(7, item.Quantidade);
                });

            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task GetBasketAsync_invalid_clienteId()
        {
            //arrange
            string clienteId = "";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act - assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => repository.GetBasketAsync(clienteId));
        }

        [Fact]
        public async Task GetBasketAsync_clienteId_NotFound()
        {
            //arrange
            var json = @"{
                  ""ClienteId"": ""123"",
                  ""Itens"": []
                }";

            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        It.IsAny<RedisValue>(),
                        null,
                        When.Always,
                        CommandFlags.None
                    ))
               .ReturnsAsync(true)
               .Verifiable();

            databaseMock.SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync("")
                    .ReturnsAsync(json);                 


            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            var basketCliente = await repository.GetBasketAsync(clienteId);

            //assert
            Assert.Equal(clienteId, basketCliente.ClienteId);
            Assert.Empty(basketCliente.Itens);
            databaseMock.Verify();
            redisMock.Verify();
        }
        #endregion

        #region AddBasketAsync
        [Fact]
        public async Task AddBasketAsync_success()
        {
            //arrange
            var json1 = JsonConvert.SerializeObject(new BasketCliente("123") { Itens = new List<ItemBasket> { new ItemBasket("001", "001", "produto 001", 12.34m, 1) }});
            var json2 = JsonConvert.SerializeObject(new BasketCliente("123") { Itens = new List<ItemBasket> { new ItemBasket("001", "001", "produto 001", 12.34m, 1), new ItemBasket("002", "002", "produto 002", 12.34m, 2) } });

            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        It.IsAny<RedisValue>(),
                        null,
                        When.Always,
                        CommandFlags.None
                    ))
               .ReturnsAsync(true);
            databaseMock
                .SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync("")
                .ReturnsAsync(json1)
                .ReturnsAsync(json2);

            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();

            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            ItemBasket item = new ItemBasket("002", "002", "produto 002", 12.34m, 2);

            //act
            var basketCliente = await repository.AddBasketAsync(clienteId, item);

            //assert
            Assert.Equal(clienteId, basketCliente.ClienteId);
            Assert.Collection(basketCliente.Itens,
                i =>
                {
                    Assert.Equal("001", i.ProdutoId);
                    Assert.Equal(1, i.Quantidade);
                },
                i =>
                {
                    Assert.Equal("002", i.ProdutoId);
                    Assert.Equal(2, i.Quantidade);
                });
            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task AddBasketAsync_invalid_item()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => repository.AddBasketAsync(clienteId, null));
        }

        [Fact]
        public async Task AddBasketAsync_invalid_item2()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => repository.AddBasketAsync(clienteId, new ItemBasket() { ProdutoId = "" }));
        }


        [Fact]
        public async Task AddBasketAsync_negative_qty()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => repository.AddBasketAsync(clienteId, new ItemBasket() { ProdutoId = "001", Quantidade = -1 }));
        }
        #endregion

        #region UpdateBasketAsync
        [Fact]
        public async Task UpdateBasketAsync_success()
        {
            //arrange
            var json1 = JsonConvert.SerializeObject(new BasketCliente("123") { Itens = new List<ItemBasket> { new ItemBasket("001", "001", "produto 001", 12.34m, 1) } });
            var json2 = JsonConvert.SerializeObject(new BasketCliente("123") { Itens = new List<ItemBasket> { new ItemBasket("001", "001", "produto 001", 12.34m, 2) } });

            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        It.IsAny<RedisValue>(),
                        null,
                        When.Always,
                        CommandFlags.None
                    ))
               .ReturnsAsync(true)
               .Verifiable();
            databaseMock
                .SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync("")
                .ReturnsAsync(json1)
                .ReturnsAsync(json2);

            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
               .Verifiable();

            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            var item = new UpdateQuantidadeInput("001", 2);

            //act
            var output = await repository.UpdateBasketAsync(clienteId, item);

            //assert
            Assert.Equal(clienteId, output.BasketCliente.ClienteId);
            Assert.Collection(output.BasketCliente.Itens,
                i =>
                {
                    Assert.Equal("001", i.ProdutoId);
                    Assert.Equal(2, i.Quantidade);
                });

            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task UpdateBasketAsync_invalid_item()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => repository.UpdateBasketAsync(clienteId, null));
        }

        [Fact]
        public async Task UpdateBasketAsync_invalid_item2()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => repository.UpdateBasketAsync(clienteId, new UpdateQuantidadeInput() { ProdutoId = "" }));
        }


        [Fact]
        public async Task UpdateBasketAsync_negative_qty()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => repository.UpdateBasketAsync(clienteId, new UpdateQuantidadeInput () { ProdutoId = "001", Quantidade = -1 }));
        }
        #endregion

        #region DeleteBasketAsync
        [Fact]
        public async Task DeleteBasketAsync_success()
        {
            //arrange
            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true)
                .Verifiable();
            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();
            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            bool result = await repository.DeleteBasketAsync(clienteId);

            //assert
            Assert.True(result);
            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task DeleteBasketAsync_failure()
        {
            //arrange
            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(false)
               .Verifiable();
            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
               .Verifiable();

            var repository
                = new RedisBasketRepository(loggerMock.Object, redisMock.Object);

            //act
            bool result = await repository.DeleteBasketAsync(clienteId);

            //assert
            Assert.False(result);
            databaseMock.Verify();
            redisMock.Verify();
        }
        #endregion
    }
}
