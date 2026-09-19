using Microsoft.EntityFrameworkCore;

public class BasketService : IBasketService
{
    private readonly OrderManagementDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public BasketService(
        OrderManagementDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<BasketResponseDto> GetMyBasket()
    {
        var customerId = await GetCurrentCustomerId();

        return await GetBasketResponse(customerId);
    }

    public async Task<BasketResponseDto> AddItem(
        AddBasketItemRequestDto dto)
    {
        var customerId = await GetCurrentCustomerId();

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

        if (product == null)
        {
            throw new ProductNotFoundException(
                $"Product {dto.ProductId} was not found.");
        }

        if (!product.IsActive)
        {
            throw new BadRequestException(
                $"Product {product.Id} is not currently available.");
        }

        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b =>
                b.CustomerId == customerId);

        if (basket == null)
        {
            basket = new Basket
            {
                CustomerId = customerId
            };

            _context.Baskets.Add(basket);
        }

        var existingItem = basket.Items
            .FirstOrDefault(bi =>
                bi.ProductId == dto.ProductId);

        if (existingItem == null)
        {
            if (BasketRules.ExceedsStock(
                dto.Quantity,
                product.Stock))
            {
                throw new BadRequestException(
                    $"Only {product.Stock} units of product {product.Id} are currently available.");
            }

            basket.Items.Add(new BasketItem
            {
                ProductId = product.Id,
                Quantity = dto.Quantity
            });
        }
        else
        {
            var newQuantity =
                BasketRules.CalculateNewQuantity(
                    existingItem.Quantity,
                    dto.Quantity);

            if (BasketRules.ExceedsMaximumQuantity(
                newQuantity))
            {
                throw new BadRequestException(
                    "Basket item quantity cannot exceed 100.");
            }

            if (BasketRules.ExceedsStock(
                newQuantity,
                product.Stock))
            {
                throw new BadRequestException(
                    $"Only {product.Stock} units of product {product.Id} are currently available.");
            }

            existingItem.Quantity = newQuantity;
        }

        await _context.SaveChangesAsync();

        return await GetBasketResponse(customerId);
    }

    public async Task<BasketResponseDto> UpdateQuantity(
        int itemId,
        UpdateBasketItemRequestDto dto)
    {
        var customerId = await GetCurrentCustomerId();

        var basketItem = await _context.BasketItems
            .Include(bi => bi.Basket)
            .Include(bi => bi.Product)
            .FirstOrDefaultAsync(bi =>
                bi.Id == itemId &&
                bi.Basket.CustomerId == customerId);

        if (basketItem == null)
        {
            throw new BasketItemNotFoundException(
                $"Basket item {itemId} was not found.");
        }

        if (!basketItem.Product.IsActive)
        {
            throw new BadRequestException(
                $"Product {basketItem.ProductId} is not currently available.");
        }

        if (BasketRules.ExceedsStock(
            dto.Quantity,
            basketItem.Product.Stock))
        {
            throw new BadRequestException(
                $"Only {basketItem.Product.Stock} units of product {basketItem.ProductId} are currently available.");
        }

        basketItem.Quantity = dto.Quantity;

        await _context.SaveChangesAsync();

        return await GetBasketResponse(customerId);
    }

    public async Task<BasketResponseDto> RemoveItem(
        int itemId)
    {
        var customerId = await GetCurrentCustomerId();

        var basketItem = await _context.BasketItems
            .Include(bi => bi.Basket)
            .FirstOrDefaultAsync(bi =>
                bi.Id == itemId &&
                bi.Basket.CustomerId == customerId);

        if (basketItem == null)
        {
            throw new BasketItemNotFoundException(
                $"Basket item {itemId} was not found.");
        }

        _context.BasketItems.Remove(basketItem);

        await _context.SaveChangesAsync();

        return await GetBasketResponse(customerId);
    }

    public async Task<BasketResponseDto> ClearBasket()
    {
        var customerId = await GetCurrentCustomerId();

        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b =>
                b.CustomerId == customerId);

        if (basket == null)
        {
            return new BasketResponseDto
            {
                Id = null,
                Items = [],
                TotalPrice = 0
            };
        }

        _context.BasketItems.RemoveRange(
            basket.Items);

        await _context.SaveChangesAsync();

        return await GetBasketResponse(customerId);
    }

    private async Task<int> GetCurrentCustomerId()
    {
        var customerId =
            await _currentUserService.GetCustomerId();

        if (customerId == null)
        {
            throw new CustomerNotFoundException(
                "No customer profile is linked to the current user.");
        }

        return customerId.Value;
    }

    private async Task<BasketResponseDto> GetBasketResponse(
        int customerId)
    {
        var basket = await _context.Baskets
            .AsNoTracking()
            .Where(b =>
                b.CustomerId == customerId)
            .Select(b => new BasketResponseDto
            {
                Id = b.Id,

                Items = b.Items
                    .Select(bi =>
                        new BasketItemResponseDto
                        {
                            Id = bi.Id,

                            ProductId =
                                bi.ProductId,

                            ProductName =
                                bi.Product.Name,

                            ProductSku =
                                bi.Product.Sku,

                            UnitPrice =
                                bi.Product.Price,

                            Quantity =
                                bi.Quantity,

                            LineTotal =
                                bi.Product.Price *
                                bi.Quantity,

                            AvailableStock =
                                bi.Product.Stock,

                            IsAvailable =
                                bi.Product.IsActive &&
                                bi.Product.Stock >=
                                    bi.Quantity
                        })
                    .ToList(),

                TotalPrice = b.Items
                    .Sum(bi =>
                        bi.Product.Price *
                        bi.Quantity)
            })
            .FirstOrDefaultAsync();

        if (basket == null)
        {
            return new BasketResponseDto
            {
                Id = null,
                Items = [],
                TotalPrice = 0
            };
        }

        return basket;
    }
}