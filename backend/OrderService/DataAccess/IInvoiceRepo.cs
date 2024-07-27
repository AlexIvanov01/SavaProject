using OrderService.Models;

namespace OrderService.DataAccess;

public interface IInvoiceRepo
{
    Task<IEnumerable<Invoice>> GetAllInvociesAsync(int lastId);
    Task<Invoice?> GetInvocieByIdAsync(int id);
    Task<Invoice?> GetInvocieByOrderIdAsync(Guid orderId);
    Task CreateInvocieAsync(Invoice invoice);
    Task<Invoice?> UpdateInvoiceAsync(Invoice invoice);
    Task<Invoice?> DeleteInvoiceAsync(int id);
}
