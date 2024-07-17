using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        product.ProductDateAdded = DateTime.Now;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product>? DeleteProductAsync(Guid id)
    {
        var dbProduct = await _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (dbProduct == null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        _context.Products.Attach(dbProduct);
        _context.Products.Remove(dbProduct);

        await _context.SaveChangesAsync();

        return dbProduct;
    }

    public async Task<ProductBatch>? DeleteBatchAsync(Guid id)
    {
        var dbBatch = await _context.ProductBatches
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.Id == id);

        if (dbBatch == null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        _context.ProductBatches.Attach(dbBatch);
        _context.ProductBatches.Remove(dbBatch);

        await _context.SaveChangesAsync();

        return dbBatch;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Include(a => a.Batches)
            .ToListAsync();
    }

    public async Task<Product>? GetProductAsync(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(a => a.Batches)
            .SingleOrDefaultAsync(p => p.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
        return product;
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async Task<Product>? GetProductOnlyAsync(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
        return product;
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async Task<Product>? UpdateProductAsync(Product product)
    {
        var DbProduct = await _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == product.Id);

        if (DbProduct == null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        DbProduct = product;
        _context.Entry(DbProduct).State = EntityState.Modified;
        DbProduct.ProductDateUpdated = DateTime.Now;

        foreach (var property in _context.Entry(DbProduct).Properties)
        {
            if (property.CurrentValue == null)
            {
                property.IsModified = false;
            }
        }
        _context.Entry(DbProduct).Property(p => p.Id).IsModified = false;
        _context.Entry(DbProduct).Property(p => p.ProductDateAdded).IsModified = false;

        await _context.SaveChangesAsync();

        return DbProduct;
    }

    public async Task<ProductBatch>? UpdateBatchAsync(ProductBatch batch)
    {
        var DbBatch = await _context.ProductBatches
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.Id == batch.Id);

        if (DbBatch == null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        DbBatch = batch;
        _context.Entry(DbBatch).State = EntityState.Modified;
        DbBatch.BatchDateUpdated = DateTime.Now;

        foreach (var property in _context.Entry(DbBatch).Properties)
        {
            if (property.CurrentValue == null)

            {
                property.IsModified = false;
            }
        }

        _context.Entry(DbBatch).Property(p => p.ProductId).IsModified = false;
        _context.Entry(DbBatch).Property(p => p.BatchDateAdded).IsModified = false;

        await _context.SaveChangesAsync();
        return DbBatch;
    }

    public async Task<Product>? AddBatchAsync(ProductBatch batch, Guid ProductId)
    {
        var product = await _context.Products
            .Include(a => a.Batches)
            .SingleOrDefaultAsync(p => p.Id == ProductId);

        if (product == null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        product.Batches.Add(batch);

        await _context.SaveChangesAsync();

        return product;
    }
}
