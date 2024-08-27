using OrderService.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace OrderService.DataAccess;

public interface IItemRepo
{
    Task<bool> ExternalItemExistsAsync(Guid? externalId);
    Task АddItemAsync(Item item);
    Task DeleteItemAsync(Guid? id);
    Task UpdateItemAsync(Item item);
    Task UpdateProductNameAsync(Guid? productId, string newProductName);
    Task DeleteItemsByProductIdAsync(Guid? productId);
}

