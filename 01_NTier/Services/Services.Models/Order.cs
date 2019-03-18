using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Services.Models
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

        public Order(Registration registration)
        { 
            CustomerName = registration.Name;
            CustomerEmail = registration.Email;
            CustomerPhone = registration.Phone;
            CustomerAddress = registration.Address;
            CustomerAdditionalAddress = registration.AdditionalAddress;
            CustomerDistrict = registration.District;
            CustomerCity = registration.City;
            CustomerState = registration.State;
            CustomerZipCode = registration.ZipCode;
        }

        [DataMember]
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
        [DataMember]
        [NotMapped]
        public virtual Registration Registration { get; private set; } = new Registration();
        [DataMember]
        [MinLength(5, ErrorMessage = "Nome deve ter no mínimo 5 caracteres")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        [Required(ErrorMessage = "CustomerId is required")]
        public string CustomerId { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Nome is required")]
        public string CustomerName { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Email is required")]
        public string CustomerEmail { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Telephone is required")]
        public string CustomerPhone { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Endereco is required")]
        public string CustomerAddress { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Complemento is required")]
        public string CustomerAdditionalAddress { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Bairro is required")]
        public string CustomerDistrict { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "Municipio is required")]
        public string CustomerCity { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "UF is required")]
        public string CustomerState { get; set; } = "";
        [DataMember]
        [Required(ErrorMessage = "CEP is required")]
        public string CustomerZipCode { get; set; } = "";
        [DataMember]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
