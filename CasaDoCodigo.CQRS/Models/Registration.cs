using System.ComponentModel.DataAnnotations;

namespace CasaDoCodigo.Models
{
    public class Registration : BaseModel
    {
        public Registration()
        {
        }

        public virtual Pedido Pedido { get; set; }
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

        internal void Update(Registration newRegistration)
        {
            this.District = newRegistration.District;
            this.ZipCode = newRegistration.ZipCode;
            this.AdditionalAddress = newRegistration.AdditionalAddress;
            this.Email = newRegistration.Email;
            this.Address = newRegistration.Address;
            this.City = newRegistration.City;
            this.Name = newRegistration.Name;
            this.Phone = newRegistration.Phone;
            this.State = newRegistration.State;
        }
    }
}
