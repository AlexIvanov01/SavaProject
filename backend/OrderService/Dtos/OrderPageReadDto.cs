using System;
using System.Collections.Generic;

namespace OrderService.Dtos;

public class OrderPageReadDto
{
    public IEnumerable<OrderFullReadDto> OrderFullReadDtos { get; set; } = [];
    public Guid Cursor { get; set; }
    public int Pages { get; set; }
}
