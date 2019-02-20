using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MVC.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MVC.Test
{
    public class BaseControllerTest
    {
        protected readonly Mock<IHttpContextAccessor> contextAccessorMock;
        protected readonly Mock<IIdentityParser<ApplicationUser>> appUserParserMock;
        protected readonly Mock<HttpContext> contextMock;

        public BaseControllerTest()
        {
            this.contextAccessorMock = new Mock<IHttpContextAccessor>();
            this.appUserParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            this.contextMock = new Mock<HttpContext>();
        }

        protected BasketItem GetFakeItemBasket()
        {
            var products = GetFakeProducts();
            var testProduct = products[0];
            var itemBasket = new BasketItem(testProduct.Code, testProduct.Code, testProduct.Name, testProduct.Price, 7, testProduct.ImageURL);
            return itemBasket;
        }

        protected IList<Product> GetFakeProducts()
        {
            Category categoria = new Category("categoria 001");

            return new List<Product>
            {
                new Product("001", "product 001", 12.34m, categoria.Id, categoria.Name),
                new Product("002", "product 002", 23.45m, categoria.Id, categoria.Name),
                new Product("003", "product 003", 34.56m, categoria.Id, categoria.Name)
            };
        }

        protected static void SetControllerUser(string customerId, BaseController controller)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { new Claim("sub", customerId) }
                ));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
