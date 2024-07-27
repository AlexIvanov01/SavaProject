using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Invoice
{
    [Key]
    [Required]
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
