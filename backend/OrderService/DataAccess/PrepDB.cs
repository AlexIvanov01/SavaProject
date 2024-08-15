using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace OrderService.DataAccess;

public static class PrepDB
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<OrderContext>());
        }
    }

    private static void SeedData(OrderContext context)
    {
        Log.Information("--> Attempting to apply migrations...");
        try
        {
            context.Database.Migrate();
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
