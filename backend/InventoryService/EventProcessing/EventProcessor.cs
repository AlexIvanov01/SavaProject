using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using InventoryService.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace InventoryService.EventProcessing;

public class EventProcessor : IEventProcessor
{

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    public async Task ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
    }

    private static EventType DetermineEvent(string notificationMessage)
    {
        Log.Information("--> Message: {Message} \n--> Determining Event...", notificationMessage);

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType == null)
        {
            Log.Error("--> Notification message deserialized to null.");
            return EventType.Undetermined;
        }

        switch (eventType.Event)
        {
            case "Order_Published":
                Log.Information("--> Order published event detected");
                return EventType.OrderPublished;
            case "Order_Updated":
                Log.Information("--> Order updated event detected");
                return EventType.OrderUpdated;
            case "Order_Deleted":
                Log.Information("--> Order deleted event detected");
                return EventType.OrderDeleted;
            default:
                Log.Information("--> Could not determine event type");
                return EventType.Undetermined;
        }

    }
}

enum EventType
{
    OrderPublished,
    OrderUpdated,
    OrderDeleted,
    Undetermined
}
