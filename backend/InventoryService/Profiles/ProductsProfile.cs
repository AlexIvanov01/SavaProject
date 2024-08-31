using System.Globalization;
using System.Security.Authentication;
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

        CreateMap<ProductBatch, GrpcInventoryModel>()
            .ForMember(dest => dest.ExternalProductId, opt => opt.MapFrom(src => src.ProductId.ToString()))
            .ForMember(dest => dest.ExternalBatchId, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.SellPrice.ToString()))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.ToString()))
            .ForMember(dest => dest.Lot, opt => opt.MapFrom(src => src.Lot));
    }
}

