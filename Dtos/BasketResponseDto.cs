public class BasketResponseDto
{
    public int? Id { get; set; }

    public List<BasketItemResponseDto> Items { get; set; } = [];

    public decimal TotalPrice { get; set; }
}