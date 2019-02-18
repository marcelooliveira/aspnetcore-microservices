using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models
{
    public class ProdutoViewModel
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Id { get; set; }
    }

    public class Item
    {
        public ProdutoViewModel Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
        public int Id { get; set; }
    }

    public class RegistrationViewModel : IEquatable<RegistrationViewModel>
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [MinLength(5, ErrorMessage = "Nome deve ter no mínimo 5 caracteres")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Email é obrigatório")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Phone { get; set; } = "";
        [Required(ErrorMessage = "Endereco é obrigatório")]
        public string Address { get; set; } = "";
        [Required(ErrorMessage = "Complemento é obrigatório")]
        public string AdditionalAddress { get; set; } = "";
        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string District { get; set; } = "";
        [Required(ErrorMessage = "Municipio é obrigatório")]
        public string City { get; set; } = "";
        [Required(ErrorMessage = "UF é obrigatório")]
        public string State { get; set; } = "";
        [Required(ErrorMessage = "CEP é obrigatório")]
        public string ZipCode { get; set; } = "";

        public RegistrationViewModel()
        {

        }

        public RegistrationViewModel(Registration registration)
        {
            this.Id = registration.Id;
            this.District = registration.District;
            this.ZipCode = registration.ZipCode;
            this.AdditionalAddress = registration.AdditionalAddress;
            this.Email = registration.Email;
            this.Address = registration.Address;
            this.City = registration.City;
            this.Name = registration.Name;
            this.Phone = registration.Phone;
            this.State = registration.State;
        }

        public bool Equals(RegistrationViewModel other)
        {
            if (other == null)
            {
                return false;
            }

            return
            this.Id == other.Id &&
            this.District == other.District &&
            this.ZipCode == other.ZipCode &&
            this.AdditionalAddress == other.AdditionalAddress &&
            this.Email == other.Email &&
            this.Address == other.Address &&
            this.City == other.City &&
            this.Name == other.Name &&
            this.Phone == other.Phone &&
            this.State == other.State;
        }
    }

    public class PedidoViewModel
    {
        public List<Item> Items { get; set; }
        public int RegistrationId { get; set; }
        public RegistrationViewModel Registration { get; set; }
        public int Id { get; set; }
    }
}
