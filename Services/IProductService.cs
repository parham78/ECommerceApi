public interface IProductService
{
    Task<PagedResultDto<Product>> GetAll(
    string? search,
    decimal? minPrice,
    decimal? maxPrice,
    bool? inStock,
    string? sortBy,
    string? sortDirection,
    int page,
    int pageSize);
    Task<PagedResultDto<Product>> GetAllForAdmin(
    bool? isActive,
    int page,
    int pageSize);

    Task<Product> GetById(int id);
    Task<Product> GetByIdForAdmin(int id);

    Task<Product> GetByName(string name);

    Task<List<Product>> GetExpensiveProducts(decimal minimumPrice);

    Task<Product> Create(CreateProductRequestDto dto);

    Task<Product> UpdateStock(
    int id,
    UpdateStockRequestDto dto);
    Task<Product> Update(int id, UpdateProductRequestDto dto);

    Task Delete(int id);
}