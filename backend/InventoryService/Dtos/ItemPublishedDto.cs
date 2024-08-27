using System;

namespace InventoryService.Dtos;

public class ItemPublishedDto
{
    public Guid? ExternalBatchId { get; set; }
    public Guid? ExternalProductId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Lot { get; set; }
    public string Event { get; set; } = string.Empty;
}
