using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminDashboardController
    : ControllerBase
{
    private readonly IAdminDashboardService
        _dashboardService;

    public AdminDashboardController(
        IAdminDashboardService dashboardService)
    {
        _dashboardService =
            dashboardService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult>
        GetSummary()
    {
        var summary =
            await _dashboardService
                .GetSummary();

        return Ok(summary);
    }

    [HttpGet("orders-by-status")]
    public async Task<IActionResult>
        GetOrdersByStatus()
    {
        var result =
            await _dashboardService
                .GetOrdersByStatus();

        return Ok(result);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult>
        GetRevenue(
            int months = 6)
    {
        var result =
            await _dashboardService
                .GetRevenue(months);

        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult>
        GetLowStockProducts(
            int threshold = 5,
            int take = 5)
    {
        var result =
            await _dashboardService
                .GetLowStockProducts(
                    threshold,
                    take);

        return Ok(result);
    }

    [HttpGet("recent-orders")]
    public async Task<IActionResult>
        GetRecentOrders(
            int take = 5)
    {
        var result =
            await _dashboardService
                .GetRecentOrders(
                    take);

        return Ok(result);
    }
}