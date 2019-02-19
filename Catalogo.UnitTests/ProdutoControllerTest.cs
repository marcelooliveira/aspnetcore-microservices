using Catalog.API.Controllers;
using Catalog.API.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class ProdutoControllerTest
    {
        private readonly Mock<ILogger<ProductController>> loggerMock;
        private readonly Mock<IProductQueries> produtoQueriesMock;

        public ProdutoControllerTest()
        {
            this.loggerMock = new Mock<ILogger<ProductController>>();
            this.produtoQueriesMock = new Mock<IProductQueries>();
        }

        public async Task GetProdutos_success()
        {
            //arrange
            var produtos = new List<Product>();
            var controller = new ProductController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProducts();

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<Product> catalog = Assert.IsType<List<Product>>(okObjectResult.Value);
            Assert.Collection(catalog,
                item => Assert.Equal(produtos[0].Code, catalog[0].Code),
                item => Assert.Equal(produtos[1].Code, catalog[1].Code),
                item => Assert.Equal(produtos[2].Code, catalog[2].Code)
            );
        }

        [Fact]
        public async Task GetProdutos_empty_catalog()
        {
            //arrange
            IList<Product> produtos = new List<Product>();
            produtoQueriesMock
                .Setup(q => q.GetProductsAsync(It.IsAny<string>()))
                .ReturnsAsync(produtos)
               .Verifiable();

            var controller = new ProductController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProducts();

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<Product> catalog = Assert.IsType<List<Product>>(okObjectResult.Value);
            Assert.Empty(catalog);
            produtoQueriesMock.Verify();
        }

        [Fact]
        public async Task GetProdutos_successAsync2()
        {
            //arrange
            const string produtoCodigo = "001";
            IList<Product> produtos = GetFakeProdutos();
            produtoQueriesMock
                .Setup(q => q.GetProductAsync(produtoCodigo))
                .ReturnsAsync(produtos[0])
               .Verifiable();

            var controller = new ProductController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProducts(produtoCodigo);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Product produto = Assert.IsType<Product>(okObjectResult.Value);
            Assert.Equal(produtos[0].Code, produto.Code);
            produtoQueriesMock.Verify();
        }

        [Fact]
        public async Task GetProdutos_not_found()
        {
            //arrange
            const string produtoCodigo = "001";
            var produtos = GetFakeProdutos();
            produtoQueriesMock
                .Setup(q => q.GetProductAsync(produtoCodigo))
                .ReturnsAsync((Product)null)
               .Verifiable();

            var controller = new ProductController(loggerMock.Object, produtoQueriesMock.Object);
            
            //act
            var actionResult = await controller.GetProducts(produtoCodigo);

            //assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
            produtoQueriesMock.Verify();
        }

        protected IList<Product> GetFakeProdutos()
        {
            return new List<Product>
            {
                new Product("001", "produto 001", 12.34m),
                new Product("002", "produto 002", 23.45m),
                new Product("003", "produto 003", 34.56m)
            };
        }
    }
}
