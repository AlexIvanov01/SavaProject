using System;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    public DateTime? InvoiceDate { get; set; }
    public Order Order { get; set; } = new();

    [MaxLength(50)]
    public string? InvoiceStatus { get; set; }
    public decimal? TotalAmount { get; set; }
}
