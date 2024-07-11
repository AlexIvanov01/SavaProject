using System;
using System.Collections.Generic;
using System.Linq;
using InventoryService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> items = new List<ItemDto>()
    {
        new ItemDto(Guid.NewGuid(),"Potion","Restore", 10),
        new ItemDto(Guid.NewGuid(),"Antidote","Cures", 1),
        new ItemDto(Guid.NewGuid(),"Posion","Damage", 12)
    };

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return items;
    }

    [HttpGet("{id}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
        var item = items.Where(item => item.Id == id).SingleOrDefault();

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto dto)
    {
        var item = new ItemDto(Guid.NewGuid(), dto.Name, dto.Description, dto.Price);
        items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, UpdateItemDto dto)
    {
        var existingItem = items.Where(item => item.Id == id).SingleOrDefault();

        if (existingItem == null) 
        {
            return NotFound();
        }

        var updatedItem = existingItem with
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
        };

        var index = items.FindIndex(item => item.Id == id);
        items[index] = updatedItem;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var index = items.FindIndex(item => item.Id == id);

        if (index == -1)
        {
            return NotFound();
        }

        items.RemoveAt(index);

        return NoContent();
    }

}
