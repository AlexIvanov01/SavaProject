using System.Collections.Generic;
using OrderService.Dtos;

namespace OrderService.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishNewOrder(OrderPublishedEvent order);
}
