using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryService.Models;

namespace InventoryService.DataAccess;

public interface IProductRepo
{
    Task<int> GetAllProductsCountAsync();
    Task<IEnumerable<Product>> GetAllProductsAsync(Guid? cursor, int pageSize);
    Task<Product?> GetProductAsync(Guid id);
    Task CreateProductAsync(Product product);
    Task<Product?> DeleteProductAsync(Guid id);
    Task<Product?> UpdateProductAsync(Product product);
    Task<Product?> AddBatchAsync(ProductBatch batch, Guid ProductId);
    Task<ProductBatch?> UpdateBatchAsync(ProductBatch batch);
    Task<ProductBatch?> DeleteBatchAsync(Guid id);

}
