using System;
using System.Collections.Generic;

namespace OrderService.Dtos;

public class OrderPageReadDto
{
    public IEnumerable<OrderReadDto>? OrderReadDtos { get; set; }
    public Guid Cursor { get; set; }
    public int Pages { get; set; }
}
