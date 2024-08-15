using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    public OrdersController(IOrderRepo orderRepo, IMapper mapper)
    {
        _repository = orderRepo;
        _mapper = mapper;
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

            var dtos = _mapper.Map<IEnumerable<OrderReadDto>>(orderPageItems);
            var pageDto = new OrderPageReadDto
            {
                OrderReadDtos = dtos,
                Cursor = dtos.Last().Id,
                Pages = (int)pageCount
            };

            return Ok(pageDto);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Internal server error");
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
        try
        {
            Log.Information("--> Creating an order.............");
            var orderModel = _mapper.Map<Order>(orderCreateDto);
            var custoemrExistCheck = await _repository.CreateOrderAsync(orderModel);

            if (custoemrExistCheck == null)
            {
                Log.Warning("Customer to add order to with id {Id} not found.",
                    orderModel.CustomerId);
                return NotFound();
            }

            Log.Information("Customer order created: {@OrderModel}", orderModel);

            var orderDeadDto = _mapper.Map<OrderReadDto>(orderModel);

            return CreatedAtRoute(nameof(GetOrderById), new { orderDeadDto.Id }, orderDeadDto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(Guid id, OrderUpdateDto orderUpdateDto)
    {
        try
        {
            Log.Information("--> Updating a product with id {Id}....................", id);

            var orderModel = _mapper.Map<Order>(orderUpdateDto);
            orderModel.Id = id;

            var nullCHeck = await _repository.UpdateOrderAsync(orderModel);

            if (nullCHeck == null)
            {
                Log.Warning("Order with id {Id} not found for updating.", id);
                return NotFound();
            }

            Log.Information("--> Order with id {Id} updated", id);

            return Ok(_mapper.Map<OrderReadDto>(orderModel));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            Log.Information("--> Deleting an order...........");

            var nullCheck = await _repository.DeleteOrderAsync(id);

            if (nullCheck == null)
            {
                Log.Warning("Order with id {Id} not found for deleting.", id);
                return NotFound();
            }

            Log.Information("--> Order with id {Id} deleted", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }

}
