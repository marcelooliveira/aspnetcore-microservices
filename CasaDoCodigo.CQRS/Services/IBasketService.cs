﻿using CasaDoCodigo.Models;
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
        Task<CustomerBasket> GetBasket(string userId);
        Task<CustomerBasket> AddItem(string customerId, BasketItem input);
        Task<UpdateQuantityOutput> UpdateItem(string customerId, UpdateQuantityInput input);
        Task<CustomerBasket> DefinirQuantidades(ApplicationUser applicationUser, Dictionary<string, int> quantidades);
        Task AtualizarBasket(CustomerBasket customerBasket);
        Task<bool> Checkout(string customerId, RegistryViewModel viewModel);
    }
}
