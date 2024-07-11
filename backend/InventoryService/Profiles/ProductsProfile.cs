using AutoMapper;
using InventoryService.Dtos;
using InventoryService.Models;

namespace InventoryService.Profiles;

public class ProductsProfile : Profile
{
    public ProductsProfile()
    {
        // Source -> Target
        CreateMap<Product, ProductReadDto>();
        CreateMap<ProductBatch, BatchReadDto>();
        CreateMap<ProductCreateDto, Product>();
        CreateMap<BatchCreateDto, ProductBatch>();
        CreateMap<ProductUpdateDto, Product>();
        CreateMap<BatchUpdateDto, ProductBatch>();
    }
}

