namespace OrderService.Dtos;

public class InvoicePageReadDto
{
    public IEnumerable<InvoiceReadDto>? InvoiceReadDtos { get; set; }
    public int Cursor { get; set; }
    public int Pages { get; set; }
}
