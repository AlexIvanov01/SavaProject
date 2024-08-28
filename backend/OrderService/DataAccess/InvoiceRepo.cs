using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using Serilog;

namespace OrderService.DataAccess;

public class InvoiceRepo : IInvoiceRepo
{
    private readonly OrderContext _context;

    public InvoiceRepo(OrderContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> UpdateInvoiceAsync(Invoice invoice)
    {
        var dbInvoice = await _context.Invoices
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == invoice.Id);

        if (dbInvoice == null)
        {
            return null;
        }

        dbInvoice = invoice;
        _context.Entry(dbInvoice).State = EntityState.Modified;

        foreach (var property in _context.Entry(dbInvoice).Properties)
        {
            if (property.CurrentValue == null)
            {
                property.IsModified = false;
            }
        }
        _context.Entry(dbInvoice).Property(p => p.Id).IsModified = false;
        _context.Entry(dbInvoice).Property(p => p.Order).IsModified = false;

        await _context.SaveChangesAsync();

        return dbInvoice;
    }

    public async Task<Order?> GetInvocieByOrderIdAsync(Guid orderId)
    {
        var invoice = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Include(o => o.Invoice)
                .Include(o => o.Customer)
                .SingleOrDefaultAsync();

        return invoice;
    }

    public async Task<Order?> GetInvocieByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.InvoiceId == id)
            .Include(o => o.Invoice)
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetAllInvociesAsync(int? cursor, int pageSize)
    {
        if (cursor != null)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Invoice)
                .Where(o => o.InvoiceId != null && o.InvoiceId > cursor)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .Include(o => o.Customer)
                .OrderBy(o => o.InvoiceId)
                .Take(pageSize)
                .ToArrayAsync();
        }
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Invoice)
            .Where(o => o.InvoiceId != null)
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .OrderBy(o => o.InvoiceId)
            .Take(pageSize)
            .ToArrayAsync();
    }

    public async Task<Invoice?> DeleteInvoiceAsync(int id)
    {
        var dbInvoice = await _context.Invoices
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (dbInvoice == null)
        {
            return null;
        }

        int? newInvoiceId = null;

        await _context.Orders
            .Where(i => i.InvoiceId == id)
            .ExecuteUpdateAsync(p => p.SetProperty(o => o.InvoiceId , newInvoiceId));

        _context.Invoices.Attach(dbInvoice);
        _context.Invoices.Remove(dbInvoice);

        await _context.SaveChangesAsync();

        return dbInvoice;
    }

    public async Task<Order?> CreateInvocieAsync(Invoice invoice, Guid orderId)
    {
        var order = await _context.Orders
            .SingleOrDefaultAsync(p => p.Id == orderId);

        if (order == null)
        {
            Log.Error("Invoice creation validation error: Order does not exist");
            return null;
        }

        if( order.InvoiceId != null)
        {
            Log.Error("Invoice creation validation error: Order already has an invoice");
            return null;
        }

        _context.Invoices.Add(invoice);
        order.Invoice = invoice;
        order.InvoiceId = invoice.Id;

        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<int> GetInvoiceCountAsync()
    {
        return await _context.Invoices.CountAsync();
    }
}
