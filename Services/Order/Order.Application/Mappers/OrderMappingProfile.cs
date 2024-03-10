using AutoMapper;
using Order.Application.Commands;
using Order.Application.Responses;
using Order.Core.Entities;

namespace Order.Application.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderEntity, OrderResponse>().ReverseMap();
            CreateMap<OrderEntity, CheckoutOrderCommand>().ReverseMap();
            CreateMap<OrderEntity, UpdateOrderCommand>().ReverseMap();
        }
    }
}
