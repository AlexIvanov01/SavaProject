using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using Serilog;

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
        if (order.OrderItems.Count == 0) 
        {
            Log.Error("Invalid order: Order has no items.");
            return null;
        }

        var dbCustomer = await _context.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.ExternalId == order.CustomerId);

        if (dbCustomer == null)
        {
            Log.Error("Invalid order: Customer with id {Id} was not found", order.CustomerId);
            return null;
        }

        var itemList = await _context.Items
            .AsNoTracking()
            .Select(i => i.ExternalBatchId)
            .ToListAsync();

        bool validItems = true;

        foreach (var item in order.OrderItems)
        {
            if (!itemList.Contains(item.ItemId))
            {
                validItems = false;
            }
        }

        if (!validItems)
        {
            Log.Error("Invalid Order: Order contains invalid items.");
            return null;
        }

        _context.Orders.Add(order);

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
                .Where(o => o.Id > cursor)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Include(o => o.Invoice)
                .Include(o => o.Customer)
                .OrderBy(o => o.Id)
                .Take(pageSize)
                .ToArrayAsync();
        }
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Invoice)
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
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
                .Where(o => o.CustomerId == customerId)
                .Where(o => o.Id > cursor)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Include(o => o.Customer)
                .Include(o => o.Invoice)
                .OrderBy(o => o.Id)
                .Take(pageSize)
                .ToListAsync();           
        }
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .OrderBy(o => o.Id)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .SingleOrDefaultAsync();

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

        if (dbOrder.OrderItems.Count == 0)
        {
            _context.Entry(dbOrder).Property(p => p.OrderItems).IsModified = false;
        }

        _context.Entry(dbOrder).Property(p => p.Id).IsModified = false;

        await _context.SaveChangesAsync();

        return dbOrder;
    }
}
