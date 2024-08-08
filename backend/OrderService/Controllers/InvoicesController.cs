using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<InvoiceReadDto>>> GetInvoices(
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

                var dtos = _mapper.Map<IEnumerable<InvoiceReadDto>>(invoiceItems);

                var pageCount = Math.Ceiling(await _repository.GetInvoiceCountAsync() / (float)pageSize);

                InvoicePageReadDto invoicePageReadDto = new()
                {
                    InvoiceReadDtos = dtos,
                    Cursor = dtos.Last().Id,
                    Pages = (int)pageCount
                };

                return Ok(invoicePageReadDto);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpGet("{id}", Name = "GetInvoiceById")]
        public async Task<ActionResult<InvoiceReadDto>> GetInvoiceById(int id)
        {
            try
            {
                Log.Information("--> Getting an invoice with id {Id}........", id);

                var invoiceItem = await _repository.GetInvocieByIdAsync(id);

                if (invoiceItem == null)
                {
                    Log.Warning("Invoice with id {Id} not found.", id);
                    return NotFound();
                }

                Log.Information("--> Fetched an invoice with id {Id}.", id);

                return Ok(_mapper.Map<InvoiceReadDto>(invoiceItem));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Internal server error");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpPost("{orderId}")]
        public async Task<ActionResult<InvoiceReadDto>> CreateProduct(InvoiceCreateDto invoiceCreateDto, Guid orderId)
        {
            try
            {
                Log.Information("--> Creating an invoice............");

                var invoiceModel = _mapper.Map<Invoice>(invoiceCreateDto);

                var orderItem = await _repository.CreateInvocieAsync(invoiceModel, orderId);

                if (orderItem == null)
                {
                    Log.Warning("Order with id {Id} not found to make an invoice.", orderId);
                    return NotFound();
                }

                Log.Information("--> Created invoice for order: {Id}", orderItem.Id);

                var orderReadDto = _mapper.Map<OrderReadDto>(orderItem);

                return CreatedAtRoute(nameof(OrdersController.GetOrderById), new { Id = orderReadDto.Id }, orderReadDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal server error.");
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
                Log.Error(ex, "Internal server error.");
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
                Log.Error(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }
    }
}
