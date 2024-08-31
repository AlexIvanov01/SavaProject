using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using InventoryService.DataAccess;

namespace InventoryService.SyncDataServices.Grpc;

public class GrpcInventoryService : GrpcInventory.GrpcInventoryBase
{
    private readonly IProductRepo _repository;
    private readonly IMapper _mapper;

    public GrpcInventoryService(IProductRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override async Task<InventoryRespone> GetAllItems(GetAllRequest request, ServerCallContext context)
    {
        var response = new InventoryRespone();

        var products = await _repository.GetAllProductBatchesAsync();

        foreach (var product in products)
        {
            foreach(var batch in product.Batches)
            {
                batch.Product = product;
                var item = _mapper.Map<GrpcInventoryModel>(batch);
                response.Item.Add(item);
            }
        }
        return response;
    }

}
