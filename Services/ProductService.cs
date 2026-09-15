using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;


public class ProductService : IProductService
{
    private readonly OrderManagementDbContext _context;

    public ProductService(OrderManagementDbContext context)
    {
        _context = context;
    }
    public async Task<PagedResultDto<Product>> GetAll(
    string? search,
    decimal? minPrice,
    decimal? maxPrice,
    bool? inStock,
    string? sortBy,
    string? sortDirection,
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
        var query = _context.Products
    .AsNoTracking()
    .Where(p => p.IsActive)
    .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Sku.Contains(search));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price <= maxPrice.Value);
        }

        if (inStock == true)
        {
            query = query.Where(p => p.Stock > 0);
        }
        else if (inStock == false)
        {
            query = query.Where(p => p.Stock == 0);
        }

        var totalCount = await query.CountAsync();

        if (sortBy == "price")
        {
            if (sortDirection == "desc")
            {
                query = query
                    .OrderByDescending(p => p.Price)
                    .ThenBy(p => p.Id);
            }
            else
            {
                query = query
                    .OrderBy(p => p.Price)
                    .ThenBy(p => p.Id);
            }
        }
        else if (sortBy == "name")
        {
            if (sortDirection == "desc")
            {
                query = query
                    .OrderByDescending(p => p.Name)
                    .ThenBy(p => p.Id);
            }
            else
            {
                query = query
                    .OrderBy(p => p.Name)
                    .ThenBy(p => p.Id);
            }
        }
        else
        {
            query = query.OrderBy(p => p.Id);
        }

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<Product>
        {
            Items = products,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }
    public async Task<PagedResultDto<Product>> GetAllForAdmin(
    bool? isActive,
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

        var query = _context.Products
            .AsNoTracking()
            .AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(p =>
                p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<Product>
        {
            Items = products,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }
    public async Task<Product> GetById(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
             p.Id == id &&
             p.IsActive);

        if (product is null)
        {
            throw new ProductNotFoundException(
                $"Product {id} was not found.");
        }

        return product;
    }
    public async Task<Product> GetByIdForAdmin(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            throw new ProductNotFoundException(
                $"Product {id} was not found.");
        }

        return product;
    }
    public async Task<Product> GetByName(string name)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
    p.Name == name &&
    p.IsActive);

        if (product is null)
        {
            throw new ProductNotFoundException(
                $"Product '{name}' was not found.");
        }

        return product;
    }
    public async Task<List<Product>> GetExpensiveProducts(decimal minimumPrice)
    {
        var products = await _context.Products
       .AsNoTracking()
       .Where(p =>
    p.IsActive &&
    p.Price > minimumPrice)
       .OrderBy(p => p.Price)
    .ToListAsync();

        return products;
    }
    public async Task<Product> Create(CreateProductRequestDto dto)
    {
        var normalizedSku = dto.Sku
            .Trim()
            .ToUpperInvariant();

        var skuExists = await _context.Products
            .AnyAsync(p => p.Sku.ToUpper() == normalizedSku);

        if (skuExists)
        {
            throw new ConflictException(
                $"A product with SKU '{normalizedSku}' already exists.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            Sku = normalizedSku,
            IsActive = dto.IsActive
        };

        _context.Products.Add(product);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sqlException &&
            (sqlException.Number == 2601 ||
             sqlException.Number == 2627))
        {
            throw new ConflictException(
                $"A product with SKU '{normalizedSku}' already exists.",
                ex);
        }

        return product;
    }
    public async Task<Product> UpdateStock(
    int id,
    UpdateStockRequestDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            throw new ProductNotFoundException(
                $"Product {id} was not found.");
        }

        _context.Entry(product)
            .Property(p => p.RowVersion)
            .OriginalValue = dto.RowVersion;

        product.Stock = dto.NewStock!.Value;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "The product stock was changed by another request. Reload it and try again.");
        }

        return product;
    }
    public async Task Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            throw new ProductNotFoundException(
                $"Product {id} was not found.");
        }

        _context.Products.Remove(product);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "The product was modified or deleted by another request. Reload it and try again.");
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sqlException
            && sqlException.Number == 547)
        {
            throw new ConflictException(
                "This product cannot be deleted because it is used in an order.",
                ex);
        }
    }
    public async Task<Product> Update(
    int id,
    UpdateProductRequestDto dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            throw new ProductNotFoundException(
                $"Product {id} was not found.");
        }

        var normalizedSku = dto.Sku
    .Trim()
    .ToUpperInvariant();

        var skuExists = await _context.Products
            .AnyAsync(p =>
                p.Sku.ToUpper() == normalizedSku &&
                p.Id != id);

        if (skuExists)
        {
            throw new ConflictException(
                $"A product with SKU '{normalizedSku}' already exists.");
        }
        _context.Entry(product)
        .Property(p => p.RowVersion)
        .OriginalValue = dto.RowVersion;

        product.Name = dto.Name;
        product.Sku = normalizedSku;
        product.Price = dto.Price;
        product.IsActive = dto.IsActive;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyConflictException(
                "The product was changed by another request. Please try again.");
        }
        catch (DbUpdateException ex) when (
    ex.InnerException is SqlException sqlException &&
    (sqlException.Number == 2601 ||
     sqlException.Number == 2627))
        {
            throw new ConflictException(
                $"A product with SKU '{normalizedSku}' already exists.",
                ex);
        }

        return product;
    }

}