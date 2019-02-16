using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Services;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MVC.Model.Redis;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CatalogControllerTest : BaseControllerTest
    {
        private readonly Mock<ICatalogService> catalogServiceMock;
        private readonly Mock<ILogger<CatalogController>> loggerMock;
        private readonly Mock<IUserRedisRepository> userRedisRepositoryMock;

        public CatalogControllerTest() : base()
        {
            catalogServiceMock = new Mock<ICatalogService>();
            loggerMock = new Mock<ILogger<CatalogController>>();
            userRedisRepositoryMock = new Mock<IUserRedisRepository>(); ;
    }

    [Fact]
        public async Task Index_sucesso()
        {
            //arrange
            IList<Produto> fakeProdutos = GetFakeProdutos();
            catalogServiceMock
                .Setup(s => s.GetProdutos())
                .ReturnsAsync(fakeProdutos)
               .Verifiable();

            var catalogController = 
                new CatalogController(catalogServiceMock.Object, loggerMock.Object, userRedisRepositoryMock.Object);

            //act
            var resultado = await catalogController.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsAssignableFrom<IList<Produto>>(viewResult.ViewData.Model);

            Assert.Collection(model,
                               item => Assert.Equal(fakeProdutos[0].Codigo, item.Codigo),
                               item => Assert.Equal(fakeProdutos[1].Codigo, item.Codigo),
                               item => Assert.Equal(fakeProdutos[2].Codigo, item.Codigo)
                );
            catalogServiceMock.Verify();
        }

        [Fact]
        public async Task Index_BrokenCircuitException()
        {
            //arrange
            catalogServiceMock
                .Setup(s => s.GetProdutos())
                .ThrowsAsync(new BrokenCircuitException());

            //act
            var catalogController =
                new CatalogController(catalogServiceMock.Object, loggerMock.Object, userRedisRepositoryMock.Object);

            var result = await catalogController.Index();
            var model = result as IList<Produto>;

            //assert
            Assert.Null(model);
            Assert.True(!string.IsNullOrWhiteSpace(catalogController.ViewBag.MsgServicoIndisponivel));
        }

        [Fact]
        public async Task Index_Exception()
        {
            //arrange
            catalogServiceMock
                .Setup(s => s.GetProdutos())
                .ThrowsAsync(new Exception());

            //act
            var catalogController =
                new CatalogController(catalogServiceMock.Object, loggerMock.Object, userRedisRepositoryMock.Object);

            var result = await catalogController.Index();
            var model = result as IList<Produto>;

            //assert
            Assert.Null(model);
            Assert.True(!string.IsNullOrWhiteSpace(catalogController.ViewBag.MsgServicoIndisponivel));
        }
    }
}
