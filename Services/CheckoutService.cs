using Microsoft.EntityFrameworkCore;

public class CheckoutService : ICheckoutService
{
    private readonly OrderManagementDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CheckoutService(
        OrderManagementDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<OrderResponseDto> Checkout(
    CheckoutRequestDto dto)
    {
        // 1. Who is making the request?
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        // 2. Make sure the customer still exists and is active.
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Id == customerId.Value);

        if (customer == null)
        {
            throw new CustomerNotFoundException(
                "Customer was not found.");
        }

        if (!customer.IsActive)
        {
            throw new BadRequestException(
                "Inactive customers cannot checkout.");
        }

        // 3. Load ONLY an address owned by this customer.
        var address = await _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.Id == dto.AddressId &&
                a.CustomerId == customerId.Value);

        if (address == null)
        {
            throw new AddressNotFoundException(
                $"Address {dto.AddressId} was not found.");
        }

        // 4. Load this customer's basket,
        // its items, and each item's Product.
        var basket = await _context.Baskets
            .Include(b => b.Items)
                .ThenInclude(bi => bi.Product)
            .FirstOrDefaultAsync(b =>
                b.CustomerId == customerId.Value);

        if (basket == null || basket.Items.Count == 0)
        {
            throw new BadRequestException(
                "Your basket is empty.");
        }

        foreach (var basketItem in basket.Items)
        {
            var product = basketItem.Product;

            if (!product.IsActive)
            {
                throw new BadRequestException(
                    $"Product {product.Id} is no longer available.");
            }

            if (basketItem.Quantity > product.Stock)
            {
                throw new BadRequestException(
                    $"Only {product.Stock} units of product {product.Id} are currently available.");
            }
        }

        await using var transaction =
    await _context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                CustomerId = customerId.Value,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,

                ShippingRecipientName = address.RecipientName,
                ShippingAddressLine1 = address.AddressLine1,
                ShippingAddressLine2 = address.AddressLine2,
                ShippingCity = address.City,
                ShippingProvince = address.Province,
                ShippingPostalCode = address.PostalCode,
                ShippingCountry = address.Country,
                ShippingPhoneNumber = address.PhoneNumber
            };

            decimal totalPrice = 0;

            foreach (var basketItem in basket.Items)
            {
                var product = basketItem.Product;

                // Reduce real inventory.
                product.Stock -= basketItem.Quantity;

                // Snapshot product information into the order.
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = basketItem.Quantity,
                    UnitPrice = product.Price,
                    ProductName = product.Name,
                    ProductSku = product.Sku
                };

                order.OrderItems.Add(orderItem);

                totalPrice +=
                    basketItem.Quantity * product.Price;
            }

            order.TotalPrice = totalPrice;

            _context.Orders.Add(order);

            // Keep the Basket itself,
            // but empty its items after successful checkout.
            _context.BasketItems.RemoveRange(
                basket.Items);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = customer.Name,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ShippingRecipientName = order.ShippingRecipientName,
                ShippingAddressLine1 = order.ShippingAddressLine1,
                ShippingAddressLine2 = order.ShippingAddressLine2,
                ShippingCity = order.ShippingCity,
                ShippingProvince = order.ShippingProvince,
                ShippingPostalCode = order.ShippingPostalCode,
                ShippingCountry = order.ShippingCountry,
                ShippingPhoneNumber = order.ShippingPhoneNumber,

                Items = order.OrderItems
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductSku = oi.ProductSku,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal =
                            oi.Quantity * oi.UnitPrice
                    })
                    .ToList()
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();

            throw new ConcurrencyConflictException(
                "Checkout failed because product stock changed. Please review your basket and try again.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}