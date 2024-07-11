using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using InventoryService.DataAccess;
using InventoryService.Dtos;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()
    {
        Console.WriteLine("--> Getting Products.........");

        var productItems = await _repository.GetAllProductsAsync();

        return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(productItems));
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<ActionResult<ProductReadDto>> GetProductById(Guid id)
    {
        Console.WriteLine("--> Getting a Product......");
        var productItem = await _repository.GetProductAsync(id);
        if (productItem != null)
        {
            return Ok(_mapper.Map<ProductReadDto>(productItem));
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> CreateProduct(ProductCreateDto productCreateDto)
    {
        Console.WriteLine("--> Creating a product.............");
        var productModel = _mapper.Map<Product>(productCreateDto);
        await _repository.CreateProductAsync(productModel);

        var productReadDto = _mapper.Map<ProductReadDto>(productModel);

        return CreatedAtRoute(nameof(GetProductById), new { Id = productReadDto.Id }, productReadDto);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct (Guid id, ProductUpdateDto productUpdateDto)
    {
        Console.WriteLine("--> Updating a product....................");

        var productModel = _mapper.Map<Product>(productUpdateDto);
        productModel.Id = id;

        await _repository.UpdateProductAsync(productModel);
        
        return Ok(_mapper.Map<ProductReadDto>(productModel));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct (Guid id)
    {
        Console.WriteLine("--> Deleting a product...........");

        await _repository.DeleteProductAsync(id);

        return NoContent();
    }
}
