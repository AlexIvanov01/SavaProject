using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryService.Models;

public class ProductBatch
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required] 
    public Guid ProductId { get; set;}
    [Required]
    [MaxLength(50)]
    public string Lot {  get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal PurchasePrice { get; set; }
    [Required]
    public decimal SellPrice { get; set; }
    [Required]
    public DateTime DateAdded { get; set; }
    [Required]
    public DateTime DateUpdated { get; set; }
    [Required]
    public DateTime ExpirationDate {  get; set; }
    [Required]
    public Product Product { get; set; }
}
