using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly OrderManagementDbContext _context;

    public CategoryService(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponseDto>> GetAll()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ProductCount = c.Products.Count(p => p.IsActive)
            })
            .ToListAsync();
    }
}