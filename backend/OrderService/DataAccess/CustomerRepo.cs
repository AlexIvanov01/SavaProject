using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.DataAccess;

public class CustomerRepo : ICustomerRepo
{
    private readonly OrderContext _context;

    public CustomerRepo(OrderContext context)
    {
        _context = context;
    }
    public async Task АddCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetCustomerAsync(Guid id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);

        return customer;
    }

    public async Task<Customer?> UpdateCustomerAsync(Customer customer)
    {
        var dbCustomer = await _context.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == customer.Id);

        if (dbCustomer == null)
        {
            return null;
        }

        dbCustomer = customer;
        _context.Entry(dbCustomer).State = EntityState.Modified;

        foreach (var property in _context.Entry(dbCustomer).Properties)
        {
            if (property.CurrentValue == null)
            {
                property.IsModified = false;
            }
        }
        _context.Entry(dbCustomer).Property(p => p.Id).IsModified = false;

        await _context.SaveChangesAsync();

        return dbCustomer;
    }

    public async Task<Customer?> DeleteCustomerAsync(Guid id)
    {
        var dbCustomer = await _context.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (dbCustomer == null)
        {
            return null;
        }

        _context.Customers.Attach(dbCustomer);
        _context.Customers.Remove(dbCustomer);

        await _context.SaveChangesAsync();

        return dbCustomer;
    }
}
