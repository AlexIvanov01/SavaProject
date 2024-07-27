using System.ComponentModel.DataAnnotations;
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
        [Range(1, 100)] int page = 1,
        Guid? customerId = null)
    {
        try
        {
            Log.Information("--> Getting all orders from page {Page}", page);

            int pageItems = 2;

            IEnumerable<Order> orderPageItems;

            if (customerId == null)
            {
                orderPageItems = await _repository.GetAllOrdersAsync(page, pageItems);
            }
            else
            {
                orderPageItems = await _repository.GetAllOrdersByCustomerIdAsync(
                    customerId,
                    page,
                    pageItems);
            }

            if (!orderPageItems.Any())
            {
                Log.Warning("--> No orderds were found on page {Page}", page);
                return NotFound();
            }

            var pageCount = Math.Ceiling(
                await _repository.GetOrderCountAsync() / (float)pageItems);

            var dtos = _mapper.Map<IEnumerable<OrderReadDto>>(orderPageItems);
            var pageDto = new OrderPageReadDto
            {
                OrderReadDtos = dtos,
                CurrentPage = page,
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

            Log.Information("Customer created: {@OrderModel}", orderModel);

            var orderDeadDto = _mapper.Map<OrderReadDto>(orderModel);

            return CreatedAtRoute(nameof(GetOrderById),
                new { Id = orderDeadDto.Id }, orderDeadDto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }

}
