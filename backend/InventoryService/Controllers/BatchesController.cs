using System.Threading.Tasks;
using System;
using AutoMapper;
using InventoryService.DataAccess;
using InventoryService.Dtos;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using InventoryService.AsyncDataServices;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchesController : ControllerBase
    {
        private readonly IProductRepo _repository;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public BatchesController(IProductRepo repository, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }
        
        [HttpPost("{ProductId}")]
        public async Task<ActionResult<BatchReadDto>> CreateBatch(BatchCreateDto batchCreateDto, Guid ProductId)
        {
            Product? productItem;
            ProductBatch batchModel;
            ProductReadDto productReadDto;

            try
            {
                Log.Information("--> Creating a batch............");

                batchModel = _mapper.Map<ProductBatch>(batchCreateDto);

                productItem = await _repository.AddBatchAsync(batchModel, ProductId);

                if (productItem == null)
                {
                    Log.Warning("--> Product with id {Id} not found to add batch to.", ProductId);
                    return NotFound();
                }

                Log.Information("--> Created batch for product: {Id}", productItem.Id);

                productReadDto = _mapper.Map<ProductReadDto>(productItem);

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occured.");
            }

            //Send Async Message
            try
            {
                ItemPublishedDto itemPublishedDto = new()
                    {
                        ExternalBatchId = batchModel.Id,
                        ExternalProductId = ProductId,
                        Name = productItem.Name,
                        Lot = batchCreateDto.Lot,
                        Price = batchCreateDto.SellPrice,
                        ExpirationDate = batchCreateDto.ExpirationDate,
                        Event = "Item_Published"
                    };
                _messageBusClient.PublishNewItem(itemPublishedDto);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error with publishing item message: {Message}", ex.Message);
            }

            return CreatedAtRoute(nameof(ProductsController.GetProductById), new { Id = productReadDto.Id }, productReadDto);
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(Guid id, BatchUpdateDto productBatchDto)
        {
            ProductBatch batchModel;
            try
            {
                Log.Information("--> Updating batch.............");

                batchModel = _mapper.Map<ProductBatch>(productBatchDto);
                batchModel.Id = id;

                var nullCheck = await _repository.UpdateBatchAsync(batchModel);

                if (nullCheck == null)
                {
                    Log.Warning("--> Batch with id {Id} not found for updating.", id);
                    return NotFound();
                }

                Log.Information("--> Batch uptated: {Id}", id);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occured.");
            }

            //Send Async Message
            if (productBatchDto.Lot != null ||
                productBatchDto.SellPrice != null ||
                productBatchDto.ExpirationDate != null)
            {
                try
                {
                    ItemPublishedDto itemPublishedDto = new()
                    {
                        ExternalBatchId = batchModel.Id,
                        Lot = batchModel.Lot,
                        Price = batchModel.SellPrice,
                        ExpirationDate = batchModel.ExpirationDate,
                        Event = "Item_Updated"
                    };

                    _messageBusClient.PublishNewItem(itemPublishedDto);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "--> Error with publishing item message: {Message}", ex.Message);
                }
            }

            return Ok(_mapper.Map<BatchReadDto>(batchModel));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBatch(Guid id)
        {
            try
            {
                Log.Information("--> Deleting a batch...........");

                var nullCHeck = await _repository.DeleteBatchAsync(id);
                if (nullCHeck == null)
                {
                    Log.Warning("--> Product with id {Id} not found for deleting.", id);
                    return NotFound();
                }

                Log.Information("--> Batch with id {Id} deleted", id);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "--> Internal server error: {Message}", ex.Message);
                return StatusCode(500, "An internal server error occured.");
            }

            //Send Async Message
            try
            {
                ItemPublishedDto itemPublishedDto = new()
                {
                    ExternalBatchId = id,
                    Event = "Item_Deleted"
                };

                _messageBusClient.PublishNewItem(itemPublishedDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error with publishing item message: {Message}", ex.Message);
            }

            return NoContent();
        }
    }
}
