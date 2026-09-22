public class AdminDashboardSummaryDto
{
    public decimal TotalRevenue { get; set; }

    public int TotalOrders { get; set; }

    public decimal AverageOrderValue { get; set; }

    public int ActiveProducts { get; set; }

    public int InactiveProducts { get; set; }

    public int LowStockProducts { get; set; }
}