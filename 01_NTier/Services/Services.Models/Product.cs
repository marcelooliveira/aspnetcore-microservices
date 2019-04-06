using Services.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Services.Models
{
    public class Product : BaseModel
    {
        public Product()
        {

        }

        [Required]
        [DataMember]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        [Required]
        [DataMember]
        public string Code { get; set; }
        [Required]
        [DataMember]
        public string Name { get; set; }
        [Required]
        [DataMember]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public string ImageURL { get { return $"/images/catalog/large_{Code}.jpg"; } }

        public Product(string code, string name, decimal price, Category category)
        {
            this.Code = code;
            this.Name = name;
            this.Price = price;
            this.Category = category;
        }
    }
}
