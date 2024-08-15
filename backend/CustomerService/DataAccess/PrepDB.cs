using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CustomerService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CustomerService.DataAccess;

public static class PrepDB
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<CustomerContext>());
        }
    }

    private static void SeedData(CustomerContext context)
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

        if (!context.Customers.Any())
        {
            Log.Information("--> Seeding Data.....");
            string file = File.ReadAllText("dummy_data.json");
            var people = JsonSerializer.Deserialize<List<Customer>>(file);
            context.Customers.AddRange(people);
            context.SaveChanges();
        }
        else
        {
            Log.Information("--> Data already present.");
        }
    }
}