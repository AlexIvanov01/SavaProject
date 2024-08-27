using System;

namespace OrderService.Models;

public class OrderItem
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = new Order();
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = new Item();
    public int OrderItemQuantity { get; set; }
}
