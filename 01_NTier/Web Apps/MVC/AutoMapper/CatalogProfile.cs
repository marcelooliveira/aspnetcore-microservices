using AutoMapper;
using Services.Models;

namespace MVC.AutoMapper
{
    public class CatalogProfile : Profile
    {
        public CatalogProfile()
        {
            CreateMap<Catalog.API.Queries.Product, Product>();
        }
    }
}
