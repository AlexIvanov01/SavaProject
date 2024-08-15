using System;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.DataAccess;

public interface ICustomerRepo
{
    Task<Customer?> GetCustomerAsync(Guid id);
    Task АddCustomerAsync(Customer customer);
    Task<Customer?> DeleteCustomerAsync(Guid id);
    Task<Customer?> UpdateCustomerAsync(Customer customer);
}
