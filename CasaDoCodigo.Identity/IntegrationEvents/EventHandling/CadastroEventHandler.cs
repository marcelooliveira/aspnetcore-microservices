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
    public class CadastroEventHandler : BaseEventHandler<RegistryEvent, CadastroCommand>, IHandleMessages<RegistryEvent>
    {
        public CadastroEventHandler(IMediator mediator, ILogger<CadastroEventHandler> logger)
            : base(mediator, logger)
        {
        }

        protected override CadastroCommand GetCommand(RegistryEvent message)
        {
            return new CadastroCommand(message.UsuarioId, message.Nome, message.Email, message.Telefone, message.Endereco, message.Complemento, message.Bairro, message.Municipio, message.UF, message.CEP);
        }
    }
}
