using System;
using System.IO;
using InventoryService.AsyncDataServices;
using InventoryService.DataAccess;
using InventoryService.EventProcessing;
using InventoryService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProductContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
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
app.MapGrpcService<GrpcInventoryService>();

app.MapGet("/protos/inventory.proto", async context =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/inventory.proto"));
});

await PrepDB.PrepPopulation(app);

await app.RunAsync();
