using AutoMapper;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Profiles;

public class OrderProfiles : Profile
{
    public OrderProfiles()
    {
        CreateMap<Order, OrderReadDto>();     

        CreateMap<Item, ItemReadDto>();
        CreateMap<Customer, CustomerReadDto>();
        CreateMap<Invoice, InvoiceReadDto>();

        CreateMap<OrderCreateDto, Order>();
        CreateMap<InvoiceCreateDto, Invoice>();
        CreateMap<OrderUpdateDto, Order>();
        CreateMap<InvoiceUpdateDto, Invoice>();

        CreateMap<ItemPublishedDto, Item>();
        CreateMap<CustomerPublishedDto, Customer>();

        CreateMap<OrderItemCreateDto, OrderItem>();
        CreateMap<OrderItem, OrderItemReadDto>();
        CreateMap<OrderItem, OrderItemPublishedDto>();
    }
}
