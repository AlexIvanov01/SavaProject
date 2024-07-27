using OrderService.Models;

namespace OrderService.DataAccess;

public interface IOrderRepo
{
    Task<IEnumerable<Order>> GetAllOrdersAsync(int page, int pageItems);
    Task<int> GetOrderCountAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order?> GetOrderByInvoiceIdAsync(int id);
    Task<IEnumerable<Order>> GetAllOrdersByCustomerIdAsync
        (Guid? customerId, int page, int pageItems);
    Task<Order?> CreateOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(Order order);
    Task<Order?> DeleteOrderAsync(Guid id);
}
