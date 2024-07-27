namespace OrderService.Dtos;

public class OrderPageReadDto
{
    public IEnumerable<OrderReadDto>? OrderReadDtos { get; set; }
    public int CurrentPage { get; set; }
    public int Pages { get; set; }
}
