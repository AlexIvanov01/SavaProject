using System.Threading.Tasks;

namespace InventoryService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}
