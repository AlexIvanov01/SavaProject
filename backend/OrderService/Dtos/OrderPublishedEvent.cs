using System.Collections.Generic;

namespace OrderService.Dtos;

public class OrderPublishedEvent
{
    public IEnumerable<OrderItemPublishedDto> OrderItems { get; set; } = [];
    public string Event { get; set; } = string.Empty;
}
