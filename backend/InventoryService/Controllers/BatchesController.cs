using System.Threading.Tasks;
using System;
using AutoMapper;
using InventoryService.DataAccess;
using InventoryService.Dtos;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchesController : ControllerBase
    {
        private readonly IProductRepo _repository;
        private readonly IMapper _mapper;

        public BatchesController(IProductRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost("{ProductId}")]
        public async Task<ActionResult<BatchReadDto>> CreateBatch(BatchCreateDto batchCreateDto, Guid ProductId)
        {
            try
            {
                Log.Information("--> Creating a batch............");

                var batchModel = _mapper.Map<ProductBatch>(batchCreateDto);

                var productItem = await _repository.AddBatchAsync(batchModel, ProductId);

                if (productItem == null)
                {
                    Log.Warning("Product with id {Id} not found.", ProductId);
                    return NotFound();
                }

                Log.Information("--> Created batch: {Id}", productItem.Id);

                var productReadDto = _mapper.Map<ProductReadDto>(productItem);

                return CreatedAtRoute(nameof(ProductsController.GetProductById), new { Id = productReadDto.Id }, productReadDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(Guid id, BatchUpdateDto productBatchDto)
        {
            try
            {
                Log.Information("--> Updating batch.............");

                var batchModel = _mapper.Map<ProductBatch>(productBatchDto);
                batchModel.Id = id;

                var nullCheck = await _repository.UpdateBatchAsync(batchModel);

                if(nullCheck == null)
                {
                    Log.Warning("Batch with id {Id} not found.", id);
                    return NotFound();
                }

                Log.Information("--> Batch uptated: {Id}", id);

                return Ok(_mapper.Map<BatchReadDto>(batchModel));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal server error.");
                return StatusCode(500, "An internal server error occured.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBatch (Guid id)
        {
            try
            {
                Log.Information("--> Deleting a batch...........");

                var nullCHeck = await _repository.DeleteBatchAsync(id);
                if(nullCHeck == null)
                {
                    Log.Warning("Product with id {Id} not found.", id);
                    return NotFound();
                }

                Log.Information("--> Batch with id {Id} deleted", id);

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
