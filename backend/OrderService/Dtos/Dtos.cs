using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Dtos;
public record OrderReadDto(Guid Id, DateTime OrderDate, DateTime? ShippedDate,
    string ShippingAddress, string OrderStatus, CustomerReadDto Customer,
    InvoiceReadDto? Invocie, List<OrderItemReadDto> OrderItems);

public record CustomerReadDto(Guid Id, string Name, string CompanyName,
     string Email, string PhoneNumber, string Address, string City,
    string Country, string BankName, string IBAN, string BIC, string VATNumber, string UIC);
    
public record InvoiceReadDto(int Id, DateTime InvoiceDate, OrderReadDto Order,
     string InvoiceStatus, decimal TotalAmount);

public record OrderItemReadDto(Guid Id, string Name,int Quantity,
    decimal Price, string Lot, Guid OrderId);

public record InvoiceCreateDto(DateTime InvoiceDate,
     string InvoiceStatus, decimal TotalAmount);

public record OrderCreateDto(DateTime OrderDate, DateTime? ShippedDate, 
    string ShippingAddress, string OrderStatus,[Required] Guid CustomerId,
    List<OrderItemCreateDto> OrderItems);

public record OrderItemCreateDto(string Name, [Required] int Quantity, 
    [Required] decimal Price, string Lot);

public record OrderUpdateDto(DateTime? OrderDate, DateTime? ShippedDate,
    string? ShippingAddress, string? OrderStatus, Guid? CustomerId);

public record InvoiceUpdateDto(DateTime? InvoiceDate,
     string? InvoiceStatus, decimal? TotalAmount);
