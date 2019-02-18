using CasaDoCodigo.Mensagens.IntegrationEvents;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using Identity.API.Commands;
using Identity.API.Managers;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace Identity.API.IntegrationEvents.EventHandling
{
    public class RegistrationEventHandler : BaseEventHandler<RegistryEvent, RegistrationCommand>, IHandleMessages<RegistryEvent>
    {
        public RegistrationEventHandler(IMediator mediator, ILogger<RegistrationEventHandler> logger)
            : base(mediator, logger)
        {
        }

        protected override RegistrationCommand GetCommand(RegistryEvent message)
        {
            return new RegistrationCommand(message.UsuarioId, message.Nome, message.Email, message.Telefone, message.Endereco, message.Complemento, message.Bairro, message.Municipio, message.UF, message.CEP);
        }
    }
}
