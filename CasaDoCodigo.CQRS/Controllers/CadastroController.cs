﻿using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.SignalR;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CadastroController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;

        public CadastroController(
            IIdentityParser<ApplicationUser> appUserParser,
            ILogger<CadastroController> logger,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.appUserParser = appUserParser;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            await CheckUserNotificationCount();

            try
            {
                var usuario = appUserParser.Parse(HttpContext.User);
                RegistryViewModel cadastro
                    = new RegistryViewModel()
                    {
                        Bairro = usuario.Bairro,
                        CEP = usuario.CEP,
                        Complemento = usuario.Complemento,
                        Email = usuario.Email,
                        Endereco = usuario.Endereco,
                        Municipio = usuario.Municipio,
                        Nome = usuario.Nome,
                        Telefone = usuario.Telefone,
                        UF = usuario.UF
                    };

                return View(cadastro);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }
    }
}
