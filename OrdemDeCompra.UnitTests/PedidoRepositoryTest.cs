using CasaDoCodigo.Ordering;
using CasaDoCodigo.Ordering.Models;
using CasaDoCodigo.Ordering.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ordering.UnitTests
{
    public class PedidoRepositoryTest
    {
        private readonly Mock<ApplicationContext> contextoMock;

        public PedidoRepositoryTest()
        {
            this.contextoMock = new Mock<ApplicationContext>();
        }

        [Fact]
        public async Task CreateOrUpdate_Pedido_NullAsync()
        {
            //arrange
            var repo = new PedidoRepository(contextoMock.Object);

            //act+assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.CreateOrUpdate(null));
        }

        [Fact]
        public async Task CreateOrUpdate_No_Items()
        {
            //arrange
            var pedido = new Order();
            var repo = new PedidoRepository(contextoMock.Object);

            //act+assert
            await Assert.ThrowsAsync<NoItemsException>(() => repo.CreateOrUpdate(pedido));
        }

        [Theory]
        [InlineData("", "product 001", 1, 12.34)]
        [InlineData("001", "", 1, 12.34)]
        [InlineData("001", "product 001", -1, 12.34)]
        [InlineData("001", "product 001", 0, 12.34)]
        [InlineData("001", "product 001", 1, 0)]
        [InlineData("001", "product 001", 1, -20)]
        public async Task CreateOrUpdate_Invalid_Item(string codigo, string nome, int qtde, decimal preco)
        {
            //arrange
            var pedido = new Order(
                new List<OrderItem> {
                    new OrderItem("001", "product 001", 1, 12.34m),
                    new OrderItem(codigo, nome, qtde, preco)
                },
                "customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");

            var repo = new PedidoRepository(contextoMock.Object);

            //act+assert
            await Assert.ThrowsAsync<InvalidItemException>(() => repo.CreateOrUpdate(pedido));
        }

        [Theory]
        [InlineData("", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("customerId", "", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("customerId", "clienteNome", "", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("customerId", "clienteNome", "cliente@email.com", "", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("customerId", "clienteNome", "cliente@email.com", "fone", "", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "", "municipio", "uf", "12345-678")]
        [InlineData("customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "", "uf", "12345-678")]
        [InlineData("customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "", "12345-678")]
        [InlineData("customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "")]
        public async Task CreateOrUpdate_Invalid_Client_Data(string customerId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            //arrange
            var pedido = new Order(
                new List<OrderItem> {
                    new OrderItem("001", "product 001", 1, 12.34m),
                    new OrderItem("002", "product 002", 2, 23.45m)
                },
                customerId, clienteNome, clienteEmail, clienteTelefone, clienteEndereco, clienteComplemento, clienteBairro, clienteMunicipio, clienteUF, clienteCEP);

            var repo = new PedidoRepository(contextoMock.Object);

            //act+assert
            await Assert.ThrowsAsync<InvalidUserDataException>(() => repo.CreateOrUpdate(pedido));
        }

        [Fact]
        public async Task CreateOrUpdate_Success()
        {
            //arrange
            var pedido = new Order(
                new List<OrderItem> {
                    new OrderItem("001", "product 001", 1, 12.34m),
                    new OrderItem("002", "product 002", 2, 23.45m)
                },
                "customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");

            var options = new DbContextOptionsBuilder<ApplicationContext>().UseInMemoryDatabase(databaseName: "database_name").Options;

            using (var context = new ApplicationContext(options))
            {
                var pedidoEntity = await context.AddAsync(pedido);

                var repo = new PedidoRepository(context);

                //act
                Order result = await repo.CreateOrUpdate(pedido);

                //assert
                Assert.Equal(2, result.Items.Count);
                Assert.Collection(result.Items,
                    item => Assert.Equal("001", item.ProductCode),
                    item => Assert.Equal("002", item.ProductCode)
                );
            }

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetPedidos_Invalid_Client(string customerId)
        {
            //arrange
            var repository = new PedidoRepository(contextoMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetPedidos(customerId));
        }

        //public class FakeContext : DbContext
        //{
        //    public FakeContext()
        //    {

        //    }

        //    public FakeContext(DbContextOptions options) : base(options)
        //    {

        //    }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        base.OnConfiguring(optionsBuilder);
        //    }

        //    public DbSet<Pedido> Pedidos { get; set; }
        //    public DbSet<ItemPedido> ItemPedidos { get; set; }
        //}

    }
}
