using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.AsyncDataServices;
using OrderService.DataAccess;
using OrderService.Dtos;
using OrderService.Models;
using Serilog;

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepo _repository;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _messageBusClient;

    public OrdersController(IOrderRepo orderRepo, IMapper mapper, IMessageBusClient messageBusClient)
    {
        _repository = orderRepo;
        _mapper = mapper;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public async Task<ActionResult<OrderPageReadDto>> GetAllOrdersAsync(
        Guid? cursor,
        Guid? customerId,
        [Range(1, 100)] int pageSize = 10)
    {
        try
        {
            IEnumerable<Order> orderPageItems;

            if (customerId == null)
            {
                Log.Information("--> Getting {PageSize} amount of orders after cursor {Cursor}", pageSize, cursor);
                orderPageItems = await _repository.GetAllOrdersAsync(cursor, pageSize);
            }
            else
            {
                Log.Information("--> Getting {PageSize} amount of orders made by customer {Customer} after cursor {Cursor}",
                    pageSize, customerId, cursor);

                orderPageItems = await _repository.GetAllOrdersByCustomerIdAsync(
                    customerId,
                    cursor,
                    pageSize);
            }

            if (!orderPageItems.Any())
            {
                Log.Warning("--> No orderds were found after {Cursor} or cursor not found.", cursor);
                return NotFound();
            }

            var pageCount = Math.Ceiling(
                await _repository.GetOrderCountAsync() / (float)pageSize);

            List<OrderFullReadDto> dtos = [];

            foreach (var item in orderPageItems)
            {
                OrderFullReadDto dto = new()
                {
                    Id = item.Id,
                    OrderDate = item.OrderDate,
                    ShippedDate = item.ShippedDate,
                    ShippingAddress = item.ShippingAddress,
                    Customer = _mapper.Map<CustomerReadDto>(item.Customer),
                    Invoice = _mapper.Map<InvoiceReadDto>(item.Invoice)
                };

                foreach(var orderItem in item.OrderItems)
                {
                    ItemReadDto itemReadDto = new()
                    {
                        ExternalBatchId = orderItem.ItemId,
                        ExternalProductId = orderItem.Item.ExternalProductId,
                        Name = orderItem.Item.Name,
                        Price = orderItem.Item.Price,
                        Lot = orderItem.Item.Lot,
                        ExpirationDate = orderItem.Item.ExpirationDate,
                        Quantity = orderItem.OrderItemQuantity
                    };

                    dto.Items.Add(itemReadDto);
                }

                dtos.Add(dto);
            }

            var pageDto = new OrderPageReadDto
            {
                OrderFullReadDtos = dtos,
                Cursor = dtos[dtos.Count-1].Id,
                Pages = (int)pageCount
            };

            return Ok(pageDto);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpGet("{id}", Name = "GetOrderById")]
    public async Task<ActionResult<OrderReadDto>> GetOrderById(Guid id)
    {
        try
        {
            Log.Information("--> Getting an order with id {Id}........", id);

            var order = await _repository.GetOrderByIdAsync(id);

            if (order == null)
            {
                Log.Warning("Order with id {Id} not found.", id);
                return NotFound();
            }

            Log.Information("--> Fetched an order with id {Id}.", id);

            return Ok(_mapper.Map<OrderReadDto>(order));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Internal server error");
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto orderCreateDto)
    {
        OrderReadDto orderReadDto;
        Order orderModel;

        try
        {
            Log.Information("--> Creating an order.............");
            orderModel = _mapper.Map<Order>(orderCreateDto);
            var validOrder = await _repository.CreateOrderAsync(orderModel);

            if (validOrder == null)
            {
                return NotFound();
            }

            Log.Information("--> Customer order created: {Id}", orderModel.Id);

            orderReadDto = _mapper.Map<OrderReadDto>(orderModel);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
            return StatusCode(500, "An internal server error occured.");
        }

        //Send Async Message
        try
        {
            OrderPublishedEvent orderPublishedEvent = new()
            {
                OrderItems = _mapper.Map<IEnumerable<OrderItemPublishedDto>>(orderModel.OrderItems),
                Event = "Order_Published"
            };

            _messageBusClient.PublishNewOrder(orderPublishedEvent);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error with publishing order message: {Message}", ex.Message);
        }

        return CreatedAtRoute(nameof(GetOrderById), new { orderReadDto.Id }, orderReadDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(Guid id, OrderUpdateDto orderUpdateDto)
    {
        Order orderModel;
        Order preUpdateOrderModel;
        try
        {
            Log.Information("--> Updating a product with id {Id}....................", id);

            orderModel = _mapper.Map<Order>(orderUpdateDto);
            orderModel.Id = id;

            var nullCHeck = await _repository.UpdateOrderAsync(orderModel);

            if (nullCHeck == null)
            {
                return NotFound();
            }
            preUpdateOrderModel = nullCHeck;

            Log.Information("--> Order with id {Id} updated", id);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
            return StatusCode(500, "An internal server error occured.");
        }

        //Send Async Message
        try
        {
            if (orderUpdateDto.OrderItems != null && orderUpdateDto.OrderItems.Count != 0)
            {
                OrderPublishedEvent orderDeletedEvent = new()
                {
                    OrderItems = _mapper.Map<IEnumerable<OrderItemPublishedDto>>(preUpdateOrderModel.OrderItems),
                    Event = "Order_Deleted"
                };

                _messageBusClient.PublishNewOrder(orderDeletedEvent);

                OrderPublishedEvent orderPublishedEvent = new()
                {
                    OrderItems = _mapper.Map<IEnumerable<OrderItemPublishedDto>>(orderUpdateDto.OrderItems),
                    Event = "Order_Published"
                };

                _messageBusClient.PublishNewOrder(orderPublishedEvent);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error with publishing order message: {Message}", ex.Message);
        }

        return Ok(_mapper.Map<OrderReadDto>(orderModel));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        Order preDeleteOrderModel;
        try
        {
            Log.Information("--> Deleting an order...........");

            var nullCheck = await _repository.DeleteOrderAsync(id);

            if (nullCheck == null)
            {
                Log.Warning("--> Order with id {Id} not found for deleting.", id);
                return NotFound();
            }

            Log.Information("--> Order with id {Id} deleted", id);

            preDeleteOrderModel = nullCheck;

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
            return StatusCode(500, "An internal server error occured.");
        }

        //Send async Message
        try
        {
            OrderPublishedEvent orderDeletedEvent = new()
            {
                OrderItems = _mapper.Map<IEnumerable<OrderItemPublishedDto>>(preDeleteOrderModel.OrderItems),
                Event = "Order_Deleted"
            };

            _messageBusClient.PublishNewOrder(orderDeletedEvent);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error with publishing order message: {Message}", ex.Message);
        }
        return NoContent();
    }

}
