using System;
using System.Collections.Generic;
using AutoMapper;
using Grpc.Net.Client;
using InventoryService;
using Microsoft.Extensions.Configuration;
using OrderService.Models;
using Serilog;

namespace OrderService.SyncDataServices.Grpc;

public class InventoryDataClient : IInventoryDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public InventoryDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public IEnumerable<Item>? ReturnAllItems()
    {
        Log.Information("--> Calling inventory GRPC service {Service}", _configuration["GrpcInventoryService"]);

        var channel = GrpcChannel.ForAddress(_configuration["GrpcInventoryService"]);
        var client = new GrpcInventory.GrpcInventoryClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllItems(request);
            return _mapper.Map<IEnumerable<Item>>(reply.Item);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not call GRPC server : {Ex}", ex.Message);
            return null;
        }
    }
}
