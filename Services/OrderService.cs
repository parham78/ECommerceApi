using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly OrderManagementDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public OrderService(
        OrderManagementDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }


    public async Task<PagedResultDto<OrderResponseDto>> GetAll(
        int page,
        int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var query = _context.Orders
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ShippingRecipientName = o.ShippingRecipientName,
                ShippingAddressLine1 = o.ShippingAddressLine1,
                ShippingAddressLine2 = o.ShippingAddressLine2,
                ShippingCity = o.ShippingCity,
                ShippingProvince = o.ShippingProvince,
                ShippingPostalCode = o.ShippingPostalCode,
                ShippingCountry = o.ShippingCountry,
                ShippingPhoneNumber = o.ShippingPhoneNumber,

                Items = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductSku = oi.ProductSku,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            })
            .ToListAsync();

        return new PagedResultDto<OrderResponseDto>
        {
            Items = orders,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }


    public async Task<PagedResultDto<OrderResponseDto>> GetMyOrders(
        int page,
        int pageSize)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId.Value);

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ShippingRecipientName = o.ShippingRecipientName,
                ShippingAddressLine1 = o.ShippingAddressLine1,
                ShippingAddressLine2 = o.ShippingAddressLine2,
                ShippingCity = o.ShippingCity,
                ShippingProvince = o.ShippingProvince,
                ShippingPostalCode = o.ShippingPostalCode,
                ShippingCountry = o.ShippingCountry,
                ShippingPhoneNumber = o.ShippingPhoneNumber,

                Items = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductSku = oi.ProductSku,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            })
            .ToListAsync();

        return new PagedResultDto<OrderResponseDto>
        {
            Items = orders,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }


    public async Task<OrderResponseDto> GetMyOrderById(int id)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var order = await _context.Orders
            .AsNoTracking()
            .Where(o =>
                o.Id == id &&
                o.CustomerId == customerId.Value)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,

                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ShippingRecipientName = o.ShippingRecipientName,
                ShippingAddressLine1 = o.ShippingAddressLine1,
                ShippingAddressLine2 = o.ShippingAddressLine2,
                ShippingCity = o.ShippingCity,
                ShippingProvince = o.ShippingProvince,
                ShippingPostalCode = o.ShippingPostalCode,
                ShippingCountry = o.ShippingCountry,
                ShippingPhoneNumber = o.ShippingPhoneNumber,


                Items = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductSku = oi.ProductSku,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (order == null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        return order;
    }


    public async Task<OrderResponseDto> GetById(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ShippingRecipientName = o.ShippingRecipientName,
                ShippingAddressLine1 = o.ShippingAddressLine1,
                ShippingAddressLine2 = o.ShippingAddressLine2,
                ShippingCity = o.ShippingCity,
                ShippingProvince = o.ShippingProvince,
                ShippingPostalCode = o.ShippingPostalCode,
                ShippingCountry = o.ShippingCountry,
                ShippingPhoneNumber = o.ShippingPhoneNumber,

                Items = o.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductSku = oi.ProductSku,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        return order;
    }


    public async Task<OrderResponseDto> Create(
        CreateOrderRequestDto dto)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            throw new CustomerNotFoundException(
                $"Customer {dto.CustomerId} was not found.");
        }

        return await CreateForCustomer(
            dto.CustomerId,
            dto.Items);
    }


    public async Task<OrderResponseDto> CreateMyOrder(
        CreateMyOrderRequestDto dto)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var isActive = await _context.Customers
            .AnyAsync(c =>
                c.Id == customerId.Value &&
                c.IsActive);

        if (!isActive)
        {
            throw new BadRequestException(
                "This customer account is inactive.");
        }

        return await CreateForCustomer(
            customerId.Value,
            dto.Items);
    }


    private async Task<OrderResponseDto> CreateForCustomer(
        int customerId,
        List<OrderItemRequestDto> items)
    {
        var productIds = items
            .Select(i => i.ProductId)
            .ToList();

        if (productIds.Distinct().Count() != productIds.Count)
        {
            throw new BadRequestException(
                "The same product cannot appear twice in an order.");
        }

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var itemDto in items)
        {
            if (!products.ContainsKey(itemDto.ProductId))
            {
                throw new ProductNotFoundException(
                    $"Product {itemDto.ProductId} was not found.");
            }
        }

        var order = new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        decimal totalPrice = 0;

        foreach (var itemDto in items)
        {
            var product = products[itemDto.ProductId];

            if (!product.IsActive)
            {
                throw new BadRequestException(
                    $"Product {product.Id} is not currently available.");
            }

            if (product.Stock < itemDto.Quantity)
            {
                throw new InsufficientStockException(
                    $"Not enough stock for product {product.Name}.");
            }

            product.Stock -= itemDto.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                ProductName = product.Name,
                ProductSku = product.Sku
            };

            order.OrderItems.Add(orderItem);

            totalPrice +=
                itemDto.Quantity * product.Price;
        }

        order.TotalPrice = totalPrice;

        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "One or more products were modified by another request.");
        }

        return await GetById(order.Id);
    }


    private static bool IsValidStatusTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        if (currentStatus == OrderStatus.Pending)
        {
            return newStatus == OrderStatus.Processing;
        }

        if (currentStatus == OrderStatus.Processing)
        {
            return newStatus == OrderStatus.Shipped;
        }

        if (currentStatus == OrderStatus.Shipped)
        {
            return newStatus == OrderStatus.Completed;
        }

        return false;
    }


    public async Task<OrderResponseDto> ChangeStatus(
        int id,
        ChangeOrderStatusRequestDto dto)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        var isValidTransition =
            IsValidStatusTransition(
                order.Status,
                dto.Status);

        if (!isValidTransition)
        {
            throw new BadRequestException(
                $"Cannot change order status from {order.Status} to {dto.Status}.");
        }

        order.Status = dto.Status;

        await _context.SaveChangesAsync();

        return await GetById(order.Id);
    }


    public async Task<OrderResponseDto> CancelOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw new BadRequestException(
                "Only pending orders can be cancelled.");
        }

        var productIds = order.OrderItems
            .Select(oi => oi.ProductId)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in order.OrderItems)
        {
            if (!products.TryGetValue(
                item.ProductId,
                out var product))
            {
                throw new ProductNotFoundException(
                    $"Product {item.ProductId} was not found.");
            }

            product.Stock += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "The order could not be cancelled because product stock changed. Please try again.");
        }

        return await GetById(order.Id);
    }
    public async Task<OrderResponseDto> CancelMyOrder(int id)
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o =>
                o.Id == id &&
                o.CustomerId == customerId.Value);

        if (order == null)
        {
            throw new OrderNotFoundException(
                $"Order {id} was not found.");
        }

        return await CancelOrder(id);
    }
}