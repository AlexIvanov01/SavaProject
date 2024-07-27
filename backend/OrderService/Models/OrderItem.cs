namespace OrderService.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public string? Lot { get; set; }
    public Guid? OrderId { get; set; }
    public Order Order { get; set; } = new Order();
}
