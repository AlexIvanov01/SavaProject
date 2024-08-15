using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models;

public class Invoice
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    public DateTime? InvoiceDate { get; set; }

    [Required]
    public Guid? OrderId { get; set; }
    public Order Order { get; set; } = new Order();

    [MaxLength(50)]
    public string? InvoiceStatus { get; set; }

    [Required]
    public decimal? TotalAmount { get; set; }
}
