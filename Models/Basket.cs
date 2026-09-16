public class Basket
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public List<BasketItem> Items { get; set; } = [];
}