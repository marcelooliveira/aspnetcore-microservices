using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Model
{
    public class ItemBasket : IValidatableObject
    {
        public ItemBasket()
        {

        }

        public ItemBasket(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade)
        {
            Id = id;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
        }

        public string Id { get; set; }
        [Required]
        public string ProdutoId { get; set; }
        [Required]
        public string ProdutoNome { get; set; }
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
        public string UrlImagem { get { return $"/images/produtos/large_{ProdutoId}.jpg"; } }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Quantidade < 1)
            {
                results.Add(new ValidationResult("Quantidade inválida", new[] { "Quantidade" }));
            }

            return results;
        }
    }
}