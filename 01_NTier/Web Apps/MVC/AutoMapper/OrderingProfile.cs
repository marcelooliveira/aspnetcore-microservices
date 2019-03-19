using AutoMapper;
using Models.ViewModels;

namespace MVC.AutoMapper
{
    public class OrderingProfile : Profile
    {
        public OrderingProfile()
        {
            CreateMap<Ordering.Models.DTOs.OrderDTO, OrderDTO>();
            CreateMap<Ordering.Models.DTOs.OrderItemDTO, OrderItemDTO>();
        }
    }
}
