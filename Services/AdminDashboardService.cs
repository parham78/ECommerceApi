using Microsoft.EntityFrameworkCore;

public class AdminDashboardService
    : IAdminDashboardService
{
    private readonly OrderManagementDbContext _context;

    public AdminDashboardService(
        OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardSummaryDto>
        GetSummary()
    {
        var totalOrders =
            await _context.Orders
                .AsNoTracking()
                .CountAsync();

        var revenueQuery =
            _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Status != OrderStatus.Cancelled);

        var revenueOrderCount =
            await revenueQuery.CountAsync();

        var totalRevenue =
            await revenueQuery
                .SumAsync(o =>
                    (decimal?)o.TotalPrice)
            ?? 0m;

        var averageOrderValue =
    revenueOrderCount == 0
        ? 0m
        : Math.Round(
            totalRevenue / revenueOrderCount,
            2);

        var activeProducts =
            await _context.Products
                .AsNoTracking()
                .CountAsync(p =>
                    p.IsActive);

        var inactiveProducts =
            await _context.Products
                .AsNoTracking()
                .CountAsync(p =>
                    !p.IsActive);

        var lowStockProducts =
            await _context.Products
                .AsNoTracking()
                .CountAsync(p =>
                    p.IsActive &&
                    p.Stock <= 5);

        return new AdminDashboardSummaryDto
        {
            TotalRevenue =
                totalRevenue,

            TotalOrders =
                totalOrders,

            AverageOrderValue =
                averageOrderValue,

            ActiveProducts =
                activeProducts,

            InactiveProducts =
                inactiveProducts,

            LowStockProducts =
                lowStockProducts
        };
    }

    public async Task<List<OrdersByStatusDto>>
        GetOrdersByStatus()
    {
        var databaseCounts =
            await _context.Orders
                .AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(group =>
                    new OrdersByStatusDto
                    {
                        Status =
                            group.Key,

                        Count =
                            group.Count()
                    })
                .ToListAsync();

        var countsByStatus =
            databaseCounts
                .ToDictionary(
                    item => item.Status,
                    item => item.Count);

        return Enum
            .GetValues<OrderStatus>()
            .Select(status =>
                new OrdersByStatusDto
                {
                    Status = status,

                    Count =
                        countsByStatus
                            .GetValueOrDefault(
                                status)
                })
            .ToList();
    }

    public async Task<List<RevenuePointDto>>
        GetRevenue(
            int months)
    {
        if (months < 1 ||
            months > 24)
        {
            throw new BadRequestException(
                "Months must be between 1 and 24.");
        }

        var now =
            DateTime.UtcNow;

        var currentMonth =
            new DateTime(
                now.Year,
                now.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

        var startDate =
            currentMonth.AddMonths(
                -(months - 1));

        var endDate =
            currentMonth.AddMonths(1);

        var databaseResults =
            await _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Status !=
                        OrderStatus.Cancelled &&
                    o.CreatedAt >=
                        startDate &&
                    o.CreatedAt <
                        endDate)
                .GroupBy(o => new
                {
                    o.CreatedAt.Year,
                    o.CreatedAt.Month
                })
                .Select(group =>
                    new RevenuePointDto
                    {
                        Year =
                            group.Key.Year,

                        Month =
                            group.Key.Month,

                        Revenue =
                            group.Sum(o =>
                                o.TotalPrice),

                        Orders =
                            group.Count()
                    })
                .OrderBy(point =>
                    point.Year)
                .ThenBy(point =>
                    point.Month)
                .ToListAsync();

        var revenueByMonth =
            databaseResults
                .ToDictionary(
                    point =>
                        (
                            point.Year,
                            point.Month
                        ));

        var result =
            new List<RevenuePointDto>();

        for (var index = 0;
             index < months;
             index++)
        {
            var month =
                startDate.AddMonths(index);

            if (revenueByMonth.TryGetValue(
                    (
                        month.Year,
                        month.Month
                    ),
                    out var revenuePoint))
            {
                result.Add(
                    revenuePoint);

                continue;
            }

            result.Add(
                new RevenuePointDto
                {
                    Year =
                        month.Year,

                    Month =
                        month.Month,

                    Revenue =
                        0m,

                    Orders =
                        0
                });
        }

        return result;
    }

    public async Task<List<LowStockProductDto>>
        GetLowStockProducts(
            int threshold,
            int take)
    {
        if (threshold < 0)
        {
            throw new BadRequestException(
                "Stock threshold cannot be negative.");
        }

        if (take < 1 ||
            take > 50)
        {
            throw new BadRequestException(
                "Take must be between 1 and 50.");
        }

        return await _context.Products
            .AsNoTracking()
            .Where(p =>
                p.IsActive &&
                p.Stock <= threshold)
            .OrderBy(p =>
                p.Stock)
            .ThenBy(p =>
                p.Name)
            .Take(take)
            .Select(p =>
                new LowStockProductDto
                {
                    Id =
                        p.Id,

                    Name =
                        p.Name,

                    Sku =
                        p.Sku,

                    Stock =
                        p.Stock
                })
            .ToListAsync();
    }

    public async Task<List<RecentAdminOrderDto>>
        GetRecentOrders(
            int take)
    {
        if (take < 1 ||
            take > 50)
        {
            throw new BadRequestException(
                "Take must be between 1 and 50.");
        }

        return await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o =>
                o.CreatedAt)
            .ThenByDescending(o =>
                o.Id)
            .Take(take)
            .Select(o =>
                new RecentAdminOrderDto
                {
                    Id =
                        o.Id,

                    CustomerId =
                        o.CustomerId,

                    CustomerName =
                        o.Customer.Name,

                    TotalPrice =
                        o.TotalPrice,

                    Status =
                        o.Status,

                    CreatedAt =
                        o.CreatedAt
                })
            .ToListAsync();
    }
}