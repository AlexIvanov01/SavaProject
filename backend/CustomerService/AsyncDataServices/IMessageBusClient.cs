using CustomerService.Dtos;

namespace CustomerService.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishNewCustomer(CustomerPublishedDto customerPublishedDto);
}
