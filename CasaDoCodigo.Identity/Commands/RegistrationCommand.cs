using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Commands
{
    public class RegistrationCommand : IRequest<bool>
    {
        public RegistrationCommand()
        {

        }

        public RegistrationCommand(string usuarioId, string nome, string email, string telephone, string address, string additionalAddress, string district, string city, string uF, string cEP)
        {
            UsuarioId = usuarioId;
            Nome = nome;
            Email = email;
            Telephone = telephone;
            Endereco = address;
            Complemento = additionalAddress;
            Bairro = district;
            Municipio = city;
            UF = uF;
            CEP = cEP;
        }

        public string UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
    }
}
