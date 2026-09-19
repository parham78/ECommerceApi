public interface ICustomerService
{
    Task<List<CustomerResponseDto>> GetAll();

    Task<CustomerResponseDto> GetById(int id);

    Task<CustomerResponseDto> Create(
        CreateCustomerRequestDto dto);
}