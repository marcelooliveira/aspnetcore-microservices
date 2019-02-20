using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasaDoCodigo.Ordering.Models
{
    public class Order : BaseModel
    {
        public Order()
        {

        }

        public Order(List<OrderItem> items, string customerId, string customerName, string customerEmail, string customerPhone, string customerAddress, string customerAdditionalAddress, string customerDistrict, string customerCity, string customerState, string customerZipCode)
        {
            Items = items;
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            CustomerAddress = customerAddress;
            CustomerAdditionalAddress = customerAdditionalAddress;
            CustomerDistrict = customerDistrict;
            CustomerCity = customerCity;
            CustomerState = customerState;
            CustomerZipCode = customerZipCode;
        }

        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
        [MinLength(5, ErrorMessage = "Nome deve ter no mínimo 5 caracteres")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        [Required(ErrorMessage = "CustomerId é obrigatório")]
        public string CustomerId { get; set; } = "";
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string CustomerName { get; set; } = "";
        [Required(ErrorMessage = "Email é obrigatório")]
        public string CustomerEmail { get; set; } = "";
        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string CustomerPhone { get; set; } = "";
        [Required(ErrorMessage = "Endereco é obrigatório")]
        public string CustomerAddress { get; set; } = "";
        [Required(ErrorMessage = "Complemento é obrigatório")]
        public string CustomerAdditionalAddress { get; set; } = "";
        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string CustomerDistrict { get; set; } = "";
        [Required(ErrorMessage = "Municipio é obrigatório")]
        public string CustomerCity { get; set; } = "";
        [Required(ErrorMessage = "UF é obrigatório")]
        public string CustomerState { get; set; } = "";
        [Required(ErrorMessage = "CEP é obrigatório")]
        public string CustomerZipCode { get; set; } = "";
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
