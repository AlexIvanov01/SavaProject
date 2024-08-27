using System;
using System.Text;
using System.Text.Json;
using InventoryService.Dtos;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;

namespace InventoryService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        try
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            Log.Information("--> Connected to MessageBus");
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Could not connect to a message bus: {Message}", ex.Message);
        }
    }

    public void PublishNewItem(ItemPublishedDto itemPublishedDto)
    {
        var message = JsonSerializer.Serialize(itemPublishedDto);

        if (_connection.IsOpen)
        {
            Log.Information("--> RabbitMQ Connection Open, sedning message...");
            SendMessage(message);
        }
        else
        {
            Log.Information("--> RabbitMQ connection closed, not sending.");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
        Log.Information("--> We have sent {Message}", message);
    }

    public void MQDispose()
    {
        Log.Information("MessageBus Disposed");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Log.Warning("--> RabbitMQ Connection Shutdown");
    }
}
