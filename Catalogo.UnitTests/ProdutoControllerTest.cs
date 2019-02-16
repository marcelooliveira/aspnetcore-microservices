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
        private readonly Mock<ILogger<ProdutoController>> loggerMock;
        private readonly Mock<IProdutoQueries> produtoQueriesMock;

        public ProdutoControllerTest()
        {
            this.loggerMock = new Mock<ILogger<ProdutoController>>();
            this.produtoQueriesMock = new Mock<IProdutoQueries>();
        }

        public async Task GetProdutos_success()
        {
            //arrange
            var produtos = new List<Produto>();
            var controller = new ProdutoController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProdutos();

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<Produto> catalog = Assert.IsType<List<Produto>>(okObjectResult.Value);
            Assert.Collection(catalog,
                item => Assert.Equal(produtos[0].Codigo, catalog[0].Codigo),
                item => Assert.Equal(produtos[1].Codigo, catalog[1].Codigo),
                item => Assert.Equal(produtos[2].Codigo, catalog[2].Codigo)
            );
        }

        [Fact]
        public async Task GetProdutos_empty_catalog()
        {
            //arrange
            IList<Produto> produtos = new List<Produto>();
            produtoQueriesMock
                .Setup(q => q.GetProdutosAsync(It.IsAny<string>()))
                .ReturnsAsync(produtos)
               .Verifiable();

            var controller = new ProdutoController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProdutos();

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<Produto> catalog = Assert.IsType<List<Produto>>(okObjectResult.Value);
            Assert.Empty(catalog);
            produtoQueriesMock.Verify();
        }

        [Fact]
        public async Task GetProdutos_successAsync2()
        {
            //arrange
            const string produtoCodigo = "001";
            IList<Produto> produtos = GetFakeProdutos();
            produtoQueriesMock
                .Setup(q => q.GetProdutoAsync(produtoCodigo))
                .ReturnsAsync(produtos[0])
               .Verifiable();

            var controller = new ProdutoController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProdutos(produtoCodigo);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Produto produto = Assert.IsType<Produto>(okObjectResult.Value);
            Assert.Equal(produtos[0].Codigo, produto.Codigo);
            produtoQueriesMock.Verify();
        }

        [Fact]
        public async Task GetProdutos_not_found()
        {
            //arrange
            const string produtoCodigo = "001";
            var produtos = GetFakeProdutos();
            produtoQueriesMock
                .Setup(q => q.GetProdutoAsync(produtoCodigo))
                .ReturnsAsync((Produto)null)
               .Verifiable();

            var controller = new ProdutoController(loggerMock.Object, produtoQueriesMock.Object);
            
            //act
            var actionResult = await controller.GetProdutos(produtoCodigo);

            //assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
            produtoQueriesMock.Verify();
        }

        protected IList<Produto> GetFakeProdutos()
        {
            return new List<Produto>
            {
                new Produto("001", "produto 001", 12.34m),
                new Produto("002", "produto 002", 23.45m),
                new Produto("003", "produto 003", 34.56m)
            };
        }
    }
}
