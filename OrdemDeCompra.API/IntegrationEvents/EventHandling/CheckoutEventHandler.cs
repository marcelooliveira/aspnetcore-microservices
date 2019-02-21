using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents;
using CasaDoCodigo.Ordering.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Linq;

namespace CasaDoCodigo.Mensagens.EventHandling
{
    public class CheckoutEventHandler : BaseEventHandler<CheckoutEvent, CreateOrderCommand>, IHandleMessages<CheckoutEvent>
    {
        public CheckoutEventHandler(IMediator mediator, ILogger<CheckoutEventHandler> logger)
            : base(mediator, logger)
        {
        }

        protected override CreateOrderCommand GetCommand(CheckoutEvent message)
        {
            var items = message.Items.Select(
                    i => new CreateOrderCommandItem(i.ProductId, i.ProductNome, i.Quantidade, i.PrecoUnitario)
                ).ToList();

            var command = new CreateOrderCommand(items, message.UserId, message.UserName, message.Email, message.Fone, message.Endereco, message.Complemento, message.Bairro, message.Municipio, message.UF, message.Cep);
            return command;
        }
    }
}
