namespace CasaDoCodigo.Models.ViewModels
{
    public class ItemPedidoDTO
    {
        public ItemPedidoDTO()
        {

        }

        public ItemPedidoDTO(string productCodigo, string productNome, int productQuantidade, decimal productPrecoUnitario)
        {
            ProductCodigo = productCodigo;
            ProductNome = productNome;
            ProductQuantidade = productQuantidade;
            ProductPrecoUnitario = productPrecoUnitario;
        }

        public string ProductCodigo { get; set; }
        public string ProductNome { get; set; }
        public int ProductQuantidade { get; set; }
        public decimal ProductPrecoUnitario { get; set; }
        public decimal Subtotal => ProductQuantidade * ProductPrecoUnitario;
    }
}
