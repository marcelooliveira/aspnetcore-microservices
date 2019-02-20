using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using CasaDoCodigo.Ordering.Models;
using CasaDoCodigo.Ordering.Repositories;
using MediatR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CasaDoCodigo.Ordering.Commands
{
    public class CreatePedidoCommandHandler
        : IRequestHandler<IdentifiedCommand<CreatePedidoCommand, bool>, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<CreatePedidoCommandHandler> _logger;
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;
        private readonly HubConnection _connection;

        public CreatePedidoCommandHandler(
            ILogger<CreatePedidoCommandHandler> logger
            , IPedidoRepository pedidoRepository
            , IBus bus
            , IConfiguration configuration
            )
        {
            this._pedidoRepository = pedidoRepository;
            this._logger = logger;
            this._bus = bus;
            this._configuration = configuration;

            _logger.LogInformation("new CreatePedidoCommandHandler created.");

            string userNotificationHubUrl = $"{_configuration["SignalRServerUrl"]}usernotificationhub";
            
            this._connection = new HubConnectionBuilder()
                .WithUrl(userNotificationHubUrl, HttpTransportType.WebSockets)
                .Build();
            this._connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this._connection.StartAsync();
            };

            this._connection.StartAsync().GetAwaiter();
        }

        public async Task<bool> Handle(IdentifiedCommand<CreatePedidoCommand, bool> request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException("Request cannot be empty");

            var cmd = request.Command;
            var guid = request.Id;

            if (cmd == null)
                throw new ArgumentNullException("Command cannot be empty");

            if (guid == Guid.Empty)
                throw new ArgumentException("Guid cannot be empty");

            var items = cmd.Items.Select(
                    i => new OrderItem(i.ProductCodigo, i.ProductNome, i.ProductQuantidade, i.ProductPrecoUnitario)
                ).ToList();

            if (items.Count == 0)
            {
                throw new NoItemsException();
            }


            foreach (var item in items)
            {
                if (
                    string.IsNullOrWhiteSpace(item.ProductCode)
                    || string.IsNullOrWhiteSpace(item.ProductName)
                    || item.ProductQuantity <= 0
                    || item.ProductUnitPrice <= 0
                    )
                {
                    throw new InvalidItemException();
                }
            }


            if (string.IsNullOrWhiteSpace(cmd.CustomerId)
                 || string.IsNullOrWhiteSpace(cmd.ClienteNome)
                 || string.IsNullOrWhiteSpace(cmd.ClienteEmail)
                 || string.IsNullOrWhiteSpace(cmd.ClienteTelefone)
                 || string.IsNullOrWhiteSpace(cmd.ClienteEndereco)
                 || string.IsNullOrWhiteSpace(cmd.ClienteComplemento)
                 || string.IsNullOrWhiteSpace(cmd.ClienteBairro)
                 || string.IsNullOrWhiteSpace(cmd.ClienteMunicipio)
                 || string.IsNullOrWhiteSpace(cmd.ClienteUF)
                 || string.IsNullOrWhiteSpace(cmd.ClienteCEP)
                )
                throw new InvalidUserDataException();

            var pedido = new Order(items, cmd.CustomerId,
                cmd.ClienteNome, cmd.ClienteEmail, cmd.ClienteTelefone, 
                cmd.ClienteEndereco, cmd.ClienteComplemento, cmd.ClienteBairro, 
                cmd.ClienteMunicipio, cmd.ClienteUF, cmd.ClienteCEP);
            pedido.DateCreated = DateTime.Now;

            try
            {
                Order novoPedido = await this._pedidoRepository.CreateOrUpdate(pedido);

                string notificationText = $"Novo pedido gerado com sucesso: {novoPedido.Id}";

                HttpClient httpClient = new HttpClient();
                string userNotificationHubUrl = $"{_configuration["SignalRServerUrl"]}usernotificationhub";

                await this._connection.InvokeAsync("SendUserNotification",
                    $"{novoPedido.CustomerId}", notificationText);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
