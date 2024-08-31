using System;
using System.Collections.Generic;
using AutoMapper;
using Grpc.Net.Client;
using CustomerService;
using Microsoft.Extensions.Configuration;
using OrderService.Models;
using Serilog;

namespace OrderService.SyncDataServices.Grpc;

public class CustomerDataClient : ICustomerDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    public CustomerDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public IEnumerable<Customer>? ReturnAllCustomers()
    {
        Log.Information("--> Calling GRPC service {Service}", _configuration["GrpcCustomerService"]);

        var channel = GrpcChannel.ForAddress(_configuration["GrpcCustomerService"]);
        var client = new GrpcCustomer.GrpcCustomerClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllCustomers(request);
            return _mapper.Map<IEnumerable<Customer>>(reply.Customers);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not call GRPC server : {Ex}", ex.Message);
            return null;
        }
    }
}
