using System;
using System.Collections.Generic;
using System.Linq;
using InventoryService.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.DataAccess;

public static class PrepDB
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<ProductContext>());
        }
    }

    private static void SeedData(ProductContext context)
    {
        if (!context.Products.Any())
        {
            Console.WriteLine("--> Seeding data...");
            string file = System.IO.File.ReadAllText("dummy_data.json");
            var people = JsonSerializer.Deserialize<List<Product>>(file);
            context.Products.AddRange(people);
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> Data is already present");
        }
    }
}
