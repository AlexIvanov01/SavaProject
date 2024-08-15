using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Order
{
    [Key]
    [Required]  
    public Guid Id { get; set; }

    [Required]
    public DateTime? OrderDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    [MaxLength(500)]
    public string? ShippingAddress { get; set; }

    [MaxLength(50)]
    public string? OrderStatus { get; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; } = new Customer();

    public Invoice? Invoice { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
