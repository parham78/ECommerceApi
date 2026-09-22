public interface IAdminDashboardService
{
    Task<AdminDashboardSummaryDto> GetSummary();

    Task<List<OrdersByStatusDto>> GetOrdersByStatus();

    Task<List<RevenuePointDto>> GetRevenue(
        int months);

    Task<List<LowStockProductDto>> GetLowStockProducts(
        int threshold,
        int take);

    Task<List<RecentAdminOrderDto>> GetRecentOrders(
        int take);
}