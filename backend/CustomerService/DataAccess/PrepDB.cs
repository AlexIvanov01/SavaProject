using System.Text.Json;
using CustomerService.Models;
using Serilog;

namespace CustomerService.DataAccess;

public class PrepDB
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
        if (!context.Customers.Any())
        {
            Log.Information("--> Seeding Data.....");
            string file = System.IO.File.ReadAllText("dummy_data.json");
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