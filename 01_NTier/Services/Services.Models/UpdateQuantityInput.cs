using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class UpdateQuantityInput
    {
        public UpdateQuantityInput()
        {

        }

        public UpdateQuantityInput(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        [Required]
        public string ProductId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
