using System;
using System.Collections.Generic;

namespace InventoryService.Dtos;

public class ProductPageReadDto
{
    public IEnumerable<ProductReadDto>? ProductReadDtos { get; set; }
    public Guid Cursor { get; set; }
    public int Pages { get; set; }
}
