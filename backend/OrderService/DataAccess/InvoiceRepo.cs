﻿using Microsoft.EntityFrameworkCore;
using OrderService.Models;

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

        await _context.SaveChangesAsync();

        return dbInvoice;
    }

    public async Task<Invoice?> GetInvocieByOrderIdAsync(Guid orderId)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Order)
            .Include(i => i.Order.OrderItems)
            .Include(i => i.Order.Customer)
            .SingleOrDefaultAsync(p => p.OrderId == orderId);

        return invoice;
    }

    public async Task<Invoice?> GetInvocieByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);

        return invoice;
    }

    public async Task<IEnumerable<Invoice>> GetAllInvociesAsync(int lastId)
    {
        return await _context.Invoices
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Where(i => i.Id > lastId)
            .Take(3)
            .ToListAsync();
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

        _context.Invoices.Attach(dbInvoice);
        _context.Invoices.Remove(dbInvoice);

        await _context.SaveChangesAsync();

        return dbInvoice;
    }

    public async Task CreateInvocieAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }
}