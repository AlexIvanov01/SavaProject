using System;
using System.IO;
using CustomerService.AsyncDataServices;
using CustomerService.DataAccess;
using CustomerService.SyncDataServeces.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CustomerContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();

app.MapControllers();
app.MapGrpcService<GrpcCustomerService>();

app.MapGet("/protos/customer.proto", async context =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/customer.proto"));
});

await PrepDB.PrepPopulation(app);

await app.RunAsync();
