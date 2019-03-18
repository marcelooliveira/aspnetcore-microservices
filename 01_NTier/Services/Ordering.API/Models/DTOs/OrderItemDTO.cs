namespace Ordering.Models.DTOs
{
    public class OrderItemDTO
    {
        public OrderItemDTO()
        {

        }

        public OrderItemDTO(string productCode, string productName, int quantity, decimal unitPrice)
        {
            ProductCode = productCode;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
