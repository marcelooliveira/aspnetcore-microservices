using AutoMapper;
using Services.Models;

namespace MVC.AutoMapper
{
    public class CatalogProfile : Profile
    {
        public CatalogProfile()
        {
            CreateMap<Catalog.API.Queries.Product, Product>()
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => 
                    new Category(src.CategoryName) { Id = src.CategoryId })
                );
        }
    }
}
