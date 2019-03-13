﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MVC
{
    public class SessionHelper : ISessionHelper
    {
        private readonly IHttpContextAccessor contextAccessor;
        public IConfiguration Configuration { get; }

        public SessionHelper(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            this.contextAccessor = contextAccessor;
            Configuration = configuration;
        }

        public int? GetOrderId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("orderId");
        }

        public void SetOrderId(int orderId)
        {
            contextAccessor.HttpContext.Session.SetInt32("orderId", orderId);
        }

        public async Task<string> GetAccessToken(string scope)
        {
            var tokenClient = new TokenClient(Configuration["IdentityUrl"] + "connect/token", "MVC", "secret");

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);
            return tokenResponse.AccessToken;
        }

        public void SetAccessToken(string accessToken)
        {
            contextAccessor.HttpContext.Session.SetString("accessToken", accessToken);
        }
    }
}
