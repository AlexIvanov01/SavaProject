using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using InventoryService.Dtos;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.DataAccess;

public class ProductRepo : IProductRepo
{
    private readonly ProductContext _context;

    public ProductRepo(ProductContext context)
    {
        _context = context;
    }

    public async Task CreateProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var dbProduct = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        ArgumentNullException.ThrowIfNull(dbProduct);

        _context.Products.Attach(dbProduct);
        _context.Products.Remove(dbProduct);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteBatchAsync(Guid id)
    {
        var dbBatch = await _context.ProductBatches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        ArgumentNullException.ThrowIfNull(dbBatch);

        _context.ProductBatches.Remove(dbBatch);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(a => a.Batches)
            .ToListAsync();
    }

    public async Task<Product> GetProductAsync(Guid id)
    {
        return await _context.Products
            .Include(a => a.Batches)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> GetProductOnlyAsync(Guid id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateProductAsync(Product product)
    {
        var DbProduct = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        ArgumentNullException.ThrowIfNull(DbProduct);

        DbProduct = product;

        _context.Entry(DbProduct).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateBatchAsync(ProductBatch batch)
    {
        var DbBatch = await _context.ProductBatches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == batch.Id);

        ArgumentNullException.ThrowIfNull(DbBatch);

        DbBatch = batch;

        _context.Entry(DbBatch).State = EntityState.Modified;
        _context.Entry(DbBatch).Property(p => p.ProductId).IsModified = false;
        _context.Entry(DbBatch).Property(p => p.DateAdded).IsModified = false;
        _context.Entry(DbBatch).Property(p => p.DateUpdated).IsModified = false;

        await _context.SaveChangesAsync();
    }

    public async Task<Product> AddBatchAsync(ProductBatch batch, Guid ProductId)
    {
        var product = await _context.Products
            .Include(a => a.Batches)
            .FirstOrDefaultAsync(p => p.Id == ProductId);

        ArgumentNullException.ThrowIfNull(product);

        product.Batches.Add(batch);

        await _context.SaveChangesAsync();

        return product;
    }
}
