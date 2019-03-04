using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ordering.Models
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
        [Required(ErrorMessage = "CustomerId is required")]
        public string CustomerId { get; set; } = "";
        [Required(ErrorMessage = "Nome is required")]
        public string CustomerName { get; set; } = "";
        [Required(ErrorMessage = "Email is required")]
        public string CustomerEmail { get; set; } = "";
        [Required(ErrorMessage = "Telephone is required")]
        public string CustomerPhone { get; set; } = "";
        [Required(ErrorMessage = "Endereco is required")]
        public string CustomerAddress { get; set; } = "";
        [Required(ErrorMessage = "Complemento is required")]
        public string CustomerAdditionalAddress { get; set; } = "";
        [Required(ErrorMessage = "Bairro is required")]
        public string CustomerDistrict { get; set; } = "";
        [Required(ErrorMessage = "Municipio is required")]
        public string CustomerCity { get; set; } = "";
        [Required(ErrorMessage = "UF is required")]
        public string CustomerState { get; set; } = "";
        [Required(ErrorMessage = "CEP is required")]
        public string CustomerZipCode { get; set; } = "";
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
