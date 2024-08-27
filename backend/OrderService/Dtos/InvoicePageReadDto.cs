﻿using System.Collections.Generic;

namespace OrderService.Dtos;

public class InvoicePageReadDto
{
    public IEnumerable<OrderFullReadDto>? OrderReadDtos { get; set; }
    public int Cursor { get; set; }
    public int Pages { get; set; }
}
