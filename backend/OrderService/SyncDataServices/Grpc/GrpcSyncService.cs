using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.DataAccess;
using Serilog;

namespace OrderService.SyncDataServices.Grpc;

public class GrpcSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(1);

    public GrpcSyncService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_syncInterval, stoppingToken);
            try
            {
                using var serviceScope = _serviceProvider.CreateScope();
                var grpcInventoryClient = serviceScope.ServiceProvider.GetService<IInventoryDataClient>();
                var grpcCustomerClient = serviceScope.ServiceProvider.GetService<ICustomerDataClient>();

                var items = grpcInventoryClient.ReturnAllItems();
                var customers = grpcCustomerClient.ReturnAllCustomers();

                if (items != null)
                {
                    await PrepDB.SyncItems(serviceScope.ServiceProvider.GetService<IItemRepo>(), items);
                }
                if (customers != null)
                {
                    await PrepDB.SyncCustomers(serviceScope.ServiceProvider.GetService<ICustomerRepo>(), customers);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured during data synchronisation: {Ex}", ex.Message);
            }
        }
    }
}
