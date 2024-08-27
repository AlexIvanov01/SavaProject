using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace OrderService.DataAccess;

public static class PrepDB
{
    public static async Task PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            await SeedData(serviceScope.ServiceProvider.GetService<OrderContext>());
        }
    }

    private static async Task SeedData(OrderContext context)
    {
        Log.Information("--> Attempting to apply migrations...");
        try
        {
            Log.Information("--> Waiting 5 seconds for DB iniialization...");
            await Task.Delay(5000);
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "--> Could not run migrations: {Ex}", ex.Message);
        }

        //if (!context.Orders.Any())
        //{
        //    Log.Information("--> Seeding data...");
        //    string file = System.IO.File.ReadAllText("dummy_data.json");
        //    var people = JsonSerializer.Deserialize<List<Order>>(file);
        //    context.Orders.AddRange(people);
        //    context.SaveChanges();
        //}
        //else
        //{
        //    Log.Information("--> Data is already present");
        //}
    }
}
