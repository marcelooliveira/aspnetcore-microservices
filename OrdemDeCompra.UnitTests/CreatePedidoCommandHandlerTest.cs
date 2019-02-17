﻿using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.Ordering.Commands;
using CasaDoCodigo.Ordering.Models;
using CasaDoCodigo.Ordering.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ordering.UnitTests
{
    public class CreatePedidoCommandHandlerTest
    {
        private readonly Mock<ILogger<CreatePedidoCommandHandler>> loggerMock;
        private readonly Mock<IPedidoRepository> pedidoRepositoryMock;
        private readonly Mock<IBus> busMock;
        private readonly Mock<IConfiguration> configurationMock;

        public CreatePedidoCommandHandlerTest()
        {
            this.loggerMock = new Mock<ILogger<CreatePedidoCommandHandler>>();
            this.pedidoRepositoryMock = new Mock<IPedidoRepository>();
            this.busMock = new Mock<IBus>();
            this.configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task Handle_request_is_null()
        {
            //arrange
            CancellationToken token = default(System.Threading.CancellationToken);
            IdentifiedCommand<CreatePedidoCommand, bool> request = null;
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_command_is_null()
        {
            //arrange
            CancellationToken token = default(System.Threading.CancellationToken);
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(null, new Guid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_guid_is_empty()
        {
            //arrange
            CancellationToken token = default(System.Threading.CancellationToken);
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(new CreatePedidoCommand(), Guid.Empty);
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_items_is_empty()
        {
            //arrange
            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand();
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<NoItemsException>(async () => await handler.Handle(request, token));
        }

        [Theory]
        [InlineData("", "produto 001", 1, 12.34)]
        [InlineData("001", "", 1, 12.34)]
        [InlineData("001", "produto 001", 0, 12.34)]
        [InlineData("001", "produto 001", -1, 12.34)]
        [InlineData("001", "produto 001", 1, -10)]
        public async Task Handle_invalid_item(string produtoCodigo, string produtoNome, int produtoQuantidade, decimal produtoPrecoUnitario)
        {
            //arrange
            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand(new List<CreatePedidoCommandItem>
            {
                new CreatePedidoCommandItem(produtoCodigo, produtoNome, produtoQuantidade, produtoPrecoUnitario)
            }
            , "customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<InvalidItemException>(async () => await handler.Handle(request, token));
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
        public async Task Handle_invalid_user_data(string customerId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            //arrange
            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand(new List<CreatePedidoCommandItem>
            {
                new CreatePedidoCommandItem("001", "produto 001", 1, 12.34m)
            }
            , customerId, clienteNome, clienteEmail, clienteTelefone, clienteEndereco, clienteComplemento, clienteBairro, clienteMunicipio, clienteUF, clienteCEP);
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<InvalidUserDataException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_success()
        {
            //arrange
            var pedido = new Pedido(
                new List<ItemPedido> {
                    new ItemPedido("001", "produto 001", 1, 12.34m),
                    new ItemPedido("002", "produto 002", 2, 23.45m)
                },
                "customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");

            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand(new List<CreatePedidoCommandItem>
            {
                new CreatePedidoCommandItem("001", "produto 001", 1, 12.34m),
                new CreatePedidoCommandItem("002", "produto 002", 2, 23.45m)
            }
            , "customerId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            pedidoRepositoryMock
                .Setup(r => r.CreateOrUpdate(It.IsAny<Pedido>()))
                .ReturnsAsync(pedido)
                .Verifiable();

            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object, configurationMock.Object);

            //act
            bool result = await handler.Handle(request, token);

            //assert
            Assert.True(result);

            pedidoRepositoryMock.Verify();
        }
    }
}
