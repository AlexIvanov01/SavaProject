using System.Threading.Tasks;
using System;
using AutoMapper;
using InventoryService.DataAccess;
using InventoryService.Dtos;
using InventoryService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            Console.WriteLine("--> Creating a batch............");

            var batchModel = _mapper.Map<ProductBatch>(batchCreateDto);
            batchModel.DateAdded = DateTime.Now;
            batchModel.DateUpdated = DateTime.Now;

            var productItem = await _repository.AddBatchAsync(batchModel, ProductId);

            var productReadDto = _mapper.Map<ProductReadDto>(productItem);

            return CreatedAtRoute(nameof(ProductsController.GetProductById), new { Id = productReadDto.Id }, productReadDto);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateBatch(Guid Id, BatchUpdateDto productBatchDto)
        {
            Console.WriteLine("--> Updating batch.............");

            var batchModel = _mapper.Map<ProductBatch>(productBatchDto);
            batchModel.Id = Id;

            await _repository.UpdateBatchAsync(batchModel);

            return Ok(_mapper.Map<BatchReadDto>(batchModel));
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteBatch (Guid Id)
        {
            Console.WriteLine("--> Deleting a product...........");

            await _repository.DeleteBatchAsync(Id);

            return NoContent();
        }
    }
}
