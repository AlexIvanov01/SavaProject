using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace OrderService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}
