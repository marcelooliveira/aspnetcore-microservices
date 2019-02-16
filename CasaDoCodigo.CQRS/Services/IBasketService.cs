using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface IBasketService : IService
    {
        Task<BasketCliente> GetBasket(string userId);
        Task<BasketCliente> AddItem(string clienteId, ItemBasket input);
        Task<UpdateQuantidadeOutput> UpdateItem(string clienteId, UpdateQuantidadeInput input);
        Task<BasketCliente> DefinirQuantidades(ApplicationUser applicationUser, Dictionary<string, int> quantidades);
        Task AtualizarBasket(BasketCliente basketCliente);
        Task<bool> Checkout(string clienteId, CadastroViewModel viewModel);
    }
}
