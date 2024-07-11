using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryService.Models;

namespace InventoryService.Dtos;

public record ItemDto(Guid Id, string Name, string Description, double Price);

public record CreateItemDto([Required] string Name, string Description, [Range(0,1000)] double Price);

public record UpdateItemDto([Required] string Name, string Description, [Range(0, 1000)] double Price);

public record BatchReadDto(Guid Id, string Lot, int Quantity, decimal PurchasePrice, decimal SellPrice, DateTime ExpirationDate);

public record ProductReadDto(Guid Id, string Name, string Description, string Brand, string Supplier, 
    string ImageURL, string Status, string Barcode, int ReorderLevel, float Weight, List<BatchReadDto> Batches);

public record ProductCreateDto([Required] string Name, string Description, [Required] string Brand, [Required] string Supplier,
    string ImageURL, string Status, string Barcode, int ReorderLevel, float Weight);

public record BatchCreateDto([Required] string Lot, [Required] int Quantity, [Required] decimal PurchasePrice, 
    [Required] decimal SellPrice, [Required] DateTime ExpirationDate);

public record ProductUpdateDto(string Name, string Description, string Brand, string Supplier,
     string ImageURL, string Status, string Barcode, int ReorderLevel, float Weight);

public record BatchUpdateDto(string Lot, int Quantity, decimal PurchasePrice,
    decimal SellPrice, DateTime ExpirationDate);
