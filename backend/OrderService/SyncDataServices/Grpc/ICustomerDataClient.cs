using OrderService.Models;
using System.Collections.Generic;

namespace OrderService.SyncDataServices.Grpc;

public interface ICustomerDataClient
{
    IEnumerable<Customer>? ReturnAllCustomers();
}
