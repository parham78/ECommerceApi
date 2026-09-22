public class RecentAdminOrderDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}