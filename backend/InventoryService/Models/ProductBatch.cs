using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryService.Models;

public class ProductBatch
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Lot {  get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? SellPrice { get; set; }
    public DateTime BatchDateAdded { get; set; }
    public DateTime? BatchDateUpdated { get; set; }
    public DateTime? ExpirationDate {  get; set; }
    public Guid ProductId { get; set;}
    public Product? Product { get; set; }
}
