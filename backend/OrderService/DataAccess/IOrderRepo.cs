﻿using OrderService.Models;

namespace OrderService.DataAccess;

public interface IOrderRepo
{
    Task<IEnumerable<Order>> GetAllOrdersAsync(Guid? cursor, int pageSize);
    Task<int> GetOrderCountAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order?> GetOrderByInvoiceIdAsync(int? id);
    Task<IEnumerable<Order>> GetAllOrdersByCustomerIdAsync
        (Guid? customerId, Guid? cursor, int pageSize);
    Task<Order?> CreateOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(Order order);
    Task<Order?> DeleteOrderAsync(Guid id);
}
