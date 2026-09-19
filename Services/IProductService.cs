public interface IProductService
{
    Task<PagedResultDto<ProductResponseDto>> GetAll(
        string? search,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize);

    Task<PagedResultDto<AdminProductResponseDto>> GetAllForAdmin(
        bool? isActive,
        int page,
        int pageSize);

    Task<ProductResponseDto> GetById(int id);

    Task<AdminProductResponseDto> GetByIdForAdmin(int id);

    Task<AdminProductResponseDto> Create(
        CreateProductRequestDto dto);

    Task<AdminProductResponseDto> UpdateStock(
        int id,
        UpdateStockRequestDto dto);

    Task<AdminProductResponseDto> Update(
        int id,
        UpdateProductRequestDto dto);

    Task Delete(int id);
}