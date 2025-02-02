﻿using System;
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

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepo _repository;
        private readonly IMapper _mapper;

        public InvoicesController(IInvoiceRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<InvoicePageReadDto>> GetInvoices(
        int? cursor,
        [Range(1, 100)] int pageSize = 10)
        {
            try
            {
                Log.Information("--> Getting {PageSize} amount of invoices after cursor {Cursor}", pageSize, cursor);

                var invoiceItems = await _repository.GetAllInvociesAsync(cursor, pageSize);
 

                if (!invoiceItems.Any())
                {
                    Log.Warning("--> No invoices found in database after cursor {Cursor} or cursor not found.", cursor);
                    return NotFound();
                }

                Log.Information("--> Fetched {PageSize} amount of invoices from database after cursor {Cursor}.", pageSize, cursor);

                var pageCount = Math.Ceiling(await _repository.GetInvoiceCountAsync() / (float)pageSize);

                List<OrderFullReadDto> dtos = [];

                foreach (var invoice in invoiceItems)
                {
                    OrderFullReadDto dto = new()
                    {
                        Id = invoice.Order.Id,
                        OrderDate = invoice.Order.OrderDate,
                        ShippedDate = invoice.Order.ShippedDate,
                        ShippingAddress = invoice.Order.ShippingAddress,
                        Customer = _mapper.Map<CustomerReadDto>(invoice.Order.Customer),
                        Invoice = _mapper.Map<InvoiceReadDto>(invoice)
                    };

                    foreach (var orderItem in invoice.Order.OrderItems)
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

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                InvoicePageReadDto invoicePageReadDto = new()
                {
                    OrderReadDtos = dtos,
                    Cursor = dtos.Select(o => o.Invoice).Last().Id,
                    Pages = (int)pageCount
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return Ok(invoicePageReadDto);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message); ;
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpGet("{id}", Name = "GetInvoiceById")]
        public async Task<ActionResult<OrderFullReadDto>> GetInvoiceById(int id)
        {
            try
            {
                Log.Information("--> Getting an invoice with id {Id}........", id);

                var invoice = await _repository.GetInvocieByIdAsync(id);

                if (invoice == null)
                {
                    Log.Warning("Invoice with id {Id} not found.", id);
                    return NotFound();
                }

                Log.Information("--> Fetched an invoice with id {Id}.", id);

                OrderFullReadDto dto = new()
                {
                    Id = invoice.Order.Id,
                    OrderDate = invoice.Order.OrderDate,
                    ShippedDate = invoice.Order.ShippedDate,
                    ShippingAddress = invoice.Order.ShippingAddress,
                    Customer = _mapper.Map<CustomerReadDto>(invoice.Order.Customer),
                    Invoice = _mapper.Map<InvoiceReadDto>(invoice)
                };

                foreach (var orderItem in invoice.Order.OrderItems)
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
                    return Ok(dto);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpPost("{orderId}")]
        public async Task<ActionResult<InvoiceReadDto>> CreateInvoice(InvoiceCreateDto invoiceCreateDto, Guid orderId)
        {
            try
            {
                Log.Information("--> Creating an invoice............");

                var invoiceModel = _mapper.Map<Invoice>(invoiceCreateDto);

                var nullCheck = await _repository.CreateInvocieAsync(invoiceModel, orderId);

                if (nullCheck == null)
                {
                    return NotFound();
                }

                Log.Information("--> Created invoice for order: {Id}", orderId);

                var invoiceReadDto = _mapper.Map<InvoiceReadDto>(invoiceModel);

                return CreatedAtRoute(nameof(GetInvoiceById), new { Id = invoiceReadDto.Id }, invoiceReadDto);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message); ;
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, InvoiceUpdateDto invoiceUpdateDto)
        {
            try
            {
                Log.Information("--> Updating invoice.............");

                var invoiceModel = _mapper.Map<Invoice>(invoiceUpdateDto);
                invoiceModel.Id = id;

                var nullCheck = await _repository.UpdateInvoiceAsync(invoiceModel);

                if (nullCheck == null)
                {
                    Log.Warning("Invoice with id {Id} not found for updating.", id);
                    return NotFound();
                }

                Log.Information("--> Invoice uptated: {Id}", id);

                return Ok(_mapper.Map<InvoiceReadDto>(invoiceModel));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInvoice(int id)
        {
            try
            {
                Log.Information("--> Deleting a batch...........");

                var nullCHeck = await _repository.DeleteInvoiceAsync(id);
                if (nullCHeck == null)
                {
                    Log.Warning("Invoice with id {Id} not found for deleting.", id);
                    return NotFound();
                }

                Log.Information("--> Invoice with id {Id} deleted", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occured.");
            }
        }
    }
}
