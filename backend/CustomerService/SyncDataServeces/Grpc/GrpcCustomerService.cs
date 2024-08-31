using System.Threading.Tasks;
using AutoMapper;
using CustomerService.DataAccess;
using Grpc.Core;

namespace CustomerService.SyncDataServeces.Grpc;

public class GrpcCustomerService : GrpcCustomer.GrpcCustomerBase
{
    private readonly ICustomerRepo _repository;
    private readonly IMapper _mapper;

    public GrpcCustomerService(ICustomerRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override async Task<CustomerResponse> GetAllCustomers(GetAllRequest request, ServerCallContext context)
    {
        var response = new CustomerResponse();

        var customers = await _repository.GetAllCustomersAsync();

        foreach (var customer in customers)
        {
            response.Customers.Add(_mapper.Map<GrpcCustomerModel>(customer));
        }
        return response;
    }
}
