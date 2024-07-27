using AutoMapper;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Profiles;

public class OrderProfiles : Profile
{
    public OrderProfiles()
    {
        CreateMap<Order, OrderReadDto>();
        CreateMap<OrderItem, OrderItemReadDto>();
        CreateMap<Customer, CustomerReadDto>();
        CreateMap<Invoice, InvoiceReadDto>();

        CreateMap<OrderCreateDto, Order>();
        CreateMap<OrderItemCreateDto, OrderItem>();
    }
}
