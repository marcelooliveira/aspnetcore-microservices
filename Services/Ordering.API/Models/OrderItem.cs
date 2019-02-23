using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CasaDoCodigo.Ordering.Models
{
    [DataContract]
    public class OrderItem : BaseModel
    {
        [Required]
        [DataMember]
        public Order Order { get; set; }
        [Required]
        [DataMember]
        public string ProductCode { get; set; }
        [Required]
        [DataMember]
        public string ProductName { get; set; }
        [Required]
        [DataMember]
        public int ProductQuantity { get; set; }
        [DataMember]
        public decimal ProductUnitPrice { get; set; }
        public decimal Subtotal => ProductQuantity * ProductUnitPrice;

        public OrderItem()
        {

        }

        public OrderItem(string productCode, string productName, int productQuantity, decimal productUnitPrice)
        {
            ProductCode = productCode;
            ProductName = productName;
            ProductQuantity = productQuantity;
            ProductUnitPrice = productUnitPrice;
        }

        public void UpdateQuantity(int productQuantity)
        {
            ProductQuantity = productQuantity;
        }
    }
}
