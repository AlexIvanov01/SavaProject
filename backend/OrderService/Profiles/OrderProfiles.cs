using System;
using AutoMapper;
using CustomerService;
using InventoryService;
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
        CreateMap<OrderItemUpdateDto, OrderItem>();
        CreateMap<InvoiceUpdateDto, Invoice>();

        CreateMap<ItemPublishedDto, Item>();
        CreateMap<CustomerPublishedDto, Customer>();

        CreateMap<OrderItemCreateDto, OrderItem>();
        CreateMap<OrderItem, OrderItemReadDto>();

        CreateMap<OrderItem, OrderItemPublishedDto>();
        CreateMap<OrderItemUpdateDto, OrderItemPublishedDto>();

        CreateMap<GrpcInventoryModel, Item>()
            .ForMember(dest => dest.ExternalProductId, opt => opt.MapFrom(src => Guid.Parse(src.ExternalProductId)))
            .ForMember(dest => dest.ExternalBatchId, opt => opt.MapFrom(src => Guid.Parse(src.ExternalBatchId)))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => decimal.Parse(src.Price)))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => DateTime.Parse(src.ExpirationDate)));

        CreateMap<GrpcCustomerModel, Customer>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => Guid.Parse(src.ExternalId)));
    }
}
