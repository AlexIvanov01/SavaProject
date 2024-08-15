using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.DataAccess;

public class OrderRepo : IOrderRepo
{
    private readonly OrderContext _context;

    public OrderRepo(OrderContext orderContext)
    {
        _context = orderContext;
    }

    public async Task<Order?> CreateOrderAsync(Order order)
    {
        var dbCustomer = await _context.Customers
            .SingleOrDefaultAsync(c => c.Id == order.CustomerId);

        if (dbCustomer == null)
        {
            return null;
        }
        dbCustomer.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> DeleteOrderAsync(Guid id)
    {
        var dbOrder = await _context.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (dbOrder == null)
        {
            return null;
        }

        _context.Orders.Attach(dbOrder);
        _context.Orders.Remove(dbOrder);

        await _context.SaveChangesAsync();

        return dbOrder;
    }

    public async Task<int> GetOrderCountAsync()
    {
        return await _context.Orders.CountAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(Guid? cursor, int pageSize)
    {
        if (cursor != null)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Invoice)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.Id >= cursor)
                .OrderBy(o => o.Id)
                .Take(pageSize)
                .ToArrayAsync();
        }
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Invoice)
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .OrderBy(o => o.Id)
            .Take(pageSize)
            .ToArrayAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersByCustomerIdAsync 
        (Guid? customerId, Guid? cursor, int pageSize)
    {
        if (cursor != null)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Include(o => o.Invoice)
                .Where(o => o.CustomerId == customerId)
                .Where(o => o.Id > cursor)
                .OrderBy(o => o.Id)
                .Take(pageSize)
                .ToListAsync();           
        }
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .Where(o => o.CustomerId == customerId)
            .OrderBy(o => o.Id)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .SingleOrDefaultAsync(p => p.Id == id);

        return order;
    }

    public async Task<Order?> GetOrderByInvoiceIdAsync(int? id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .SingleOrDefaultAsync(p => p.Invoice.Id == id);

        return order;
    }

    public async Task<Order?> UpdateOrderAsync(Order order)
    {
        var dbOrder = await _context.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == order.Id);

        if (dbOrder == null)
        {
            return null;
        }

        dbOrder = order;
        _context.Entry(dbOrder).State = EntityState.Modified;

        foreach (var property in _context.Entry(dbOrder).Properties)
        {
            if (property.CurrentValue == null)
            {
                property.IsModified = false;
            }
        }
        _context.Entry(dbOrder).Property(p => p.Id).IsModified = false;

        await _context.SaveChangesAsync();

        return dbOrder;
    }
}
