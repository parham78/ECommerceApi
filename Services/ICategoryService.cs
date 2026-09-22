public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAll();
}