using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.DataAccess;

public interface IInvoiceRepo
{
    Task<IEnumerable<Order>> GetAllInvociesAsync(int? cursor, int pageSize);
    Task<Order?> GetInvocieByIdAsync(int id);
    Task<Order?> GetInvocieByOrderIdAsync(Guid orderId);
    Task<Order?> CreateInvocieAsync(Invoice invoice, Guid orderId);
    Task<Invoice?> UpdateInvoiceAsync(Invoice invoice);
    Task<Invoice?> DeleteInvoiceAsync(int id);
    Task<int> GetInvoiceCountAsync();
}
