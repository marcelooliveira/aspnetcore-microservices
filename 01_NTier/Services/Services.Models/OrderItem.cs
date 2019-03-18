using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Services.Models
{
    [DataContract]
    public class OrderItem : BaseModel
    {   
        [Required]
        [DataMember]
        [JsonIgnore]
        public Order Order { get; set; }
        [Required]
        [DataMember]
        public string ProductCode { get; set; }
        [Required]
        [DataMember]
        public string ProductName { get; set; }
        [Required]
        [DataMember]
        public int Quantity { get; set; }
        [Required]
        [DataMember]
		[Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;

        public OrderItem()
        {

        }

        public OrderItem(string productCode, string productName, int quantity, decimal unitPrice)
        {
            ProductCode = productCode;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public OrderItem(Order order, string productCode, string productName, int quantity, decimal unitPrice)
        {
            Order = order;
            ProductCode = productCode;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        internal void UpdateQuantity(int quantity)
        {
            Quantity = quantity;
        }
    }
}
