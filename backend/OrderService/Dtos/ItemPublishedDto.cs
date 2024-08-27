using System;

namespace OrderService.Dtos;

public class ItemPublishedDto
{
    public Guid? ExternalProductId { get; set; }
    public Guid? ExternalBatchId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Lot { get; set; }
    public string Event { get; set; } = string.Empty;
}
