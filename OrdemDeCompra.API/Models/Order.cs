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
        [MinLength(5, ErrorMessage = "Nome deve ter no m�nimo 5 caracteres")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no m�ximo 50 caracteres")]
        [Required(ErrorMessage = "CustomerId � obrigat�rio")]
        public string CustomerId { get; set; } = "";
        [Required(ErrorMessage = "Nome � obrigat�rio")]
        public string CustomerName { get; set; } = "";
        [Required(ErrorMessage = "Email � obrigat�rio")]
        public string CustomerEmail { get; set; } = "";
        [Required(ErrorMessage = "Telefone � obrigat�rio")]
        public string CustomerPhone { get; set; } = "";
        [Required(ErrorMessage = "Endereco � obrigat�rio")]
        public string CustomerAddress { get; set; } = "";
        [Required(ErrorMessage = "Complemento � obrigat�rio")]
        public string CustomerAdditionalAddress { get; set; } = "";
        [Required(ErrorMessage = "Bairro � obrigat�rio")]
        public string CustomerDistrict { get; set; } = "";
        [Required(ErrorMessage = "Municipio � obrigat�rio")]
        public string CustomerCity { get; set; } = "";
        [Required(ErrorMessage = "UF � obrigat�rio")]
        public string CustomerState { get; set; } = "";
        [Required(ErrorMessage = "CEP � obrigat�rio")]
        public string CustomerZipCode { get; set; } = "";
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
