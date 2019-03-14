using AutoMapper;
using Basket.API.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.AutoMapper
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<Basket.API.Model.CustomerBasket, CustomerBasket>();
            CreateMap<CustomerBasket, Basket.API.Model.CustomerBasket>();
            CreateMap<BasketItem, Basket.API.Model.BasketItem>();
            CreateMap<Basket.API.Model.BasketItem, BasketItem>();
        }
    }
}
