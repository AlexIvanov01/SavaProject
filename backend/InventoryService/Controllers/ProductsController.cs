using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InventoryService.DataAccess;
using InventoryService.Dtos;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InventoryService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepo _repository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts(
        Guid? cursor,
        [Range(1, 100)] int pageSize = 10)
    {
        try
        {
            Log.Information("--> Getting {PageSize} amount of products after cursor {Cursor}", pageSize, cursor);

            var productItems = await _repository.GetAllProductsAsync(cursor, pageSize);

            if(!productItems.Any())
            {
                Log.Warning("--> No products found in database after cursor {Cursor} or cursor not found.", cursor);
                return NotFound();
            }

            Log.Information("--> Fetched {PageSize} amount of products from database after cursor {Cursor}.", pageSize, cursor);

            var dtos = _mapper.Map<IEnumerable<ProductReadDto>>(productItems);

            var pageCount = Math.Ceiling(await _repository.GetAllProductsCountAsync() / (float)pageSize);

            ProductPageReadDto productPageReadDto = new()
            {
                ProductReadDtos = dtos,
                Cursor = dtos.Last().Id,
                Pages = (int)pageCount
            };

            return Ok(productPageReadDto);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<ProductReadDto>> GetProductById(Guid id)
    {
        try
        {
            Log.Information("--> Getting a product with id {Id}........", id);

            var productItem = await _repository.GetProductAsync(id);

            if (productItem == null)
            {
                Log.Warning("Product with id {Id} not found.", id);
                return NotFound();
            }

            Log.Information("--> Fetched a product with id {Id}.", id);

            return Ok(_mapper.Map<ProductReadDto>(productItem));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Internal server error");
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> CreateProduct(ProductCreateDto productCreateDto)
    {
        try
        {
            Log.Information("--> Creating a product.............");
            var productModel = _mapper.Map<Product>(productCreateDto);
            await _repository.CreateProductAsync(productModel);

            Log.Information("Product created: {@ProductModel}", productModel);

            var productReadDto = _mapper.Map<ProductReadDto>(productModel);

            return CreatedAtRoute(nameof(GetProductById), new { Id = productReadDto.Id }, productReadDto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateDto productUpdateDto)
    {
        try
        {
            Log.Information("--> Updating a product with id {Id}....................", id);

            var productModel = _mapper.Map<Product>(productUpdateDto);
            productModel.Id = id;

            var nullCHeck = await _repository.UpdateProductAsync(productModel);

            if (nullCHeck == null)
            {
                Log.Warning("Product with id {Id} not found for updating.", id);
                return NotFound();
            }

            Log.Information("--> Product with id {Id} updated", id);

            return Ok(_mapper.Map<ProductReadDto>(productModel));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            Log.Information("--> Deleting a product...........");

            var nullCheck = await _repository.DeleteProductAsync(id);


            if (nullCheck == null)
            {
                Log.Warning("Product with id {Id} not found for deleting.", id);
                return NotFound();
            }

            Log.Information("--> Product with id {Id} deleted", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error.");
            return StatusCode(500, "An internal server error occured.");
        }
    }
}
