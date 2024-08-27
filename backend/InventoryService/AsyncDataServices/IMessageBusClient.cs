using InventoryService.Dtos;

namespace InventoryService.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishNewItem(ItemPublishedDto itemPublishedDto);
}
