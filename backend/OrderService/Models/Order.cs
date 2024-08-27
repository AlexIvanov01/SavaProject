using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Order
{
    [Key]  
    public Guid Id { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    [MaxLength(500)]
    public string? ShippingAddress { get; set; }

    [MaxLength(50)]
    public string? OrderStatus { get; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public List<OrderItem> OrderItems { get; set; } = [];
}
