public interface IAddressService
{
    Task<List<AddressResponseDto>> GetMyAddresses();

    Task<AddressResponseDto> GetMyAddressById(int id);

    Task<AddressResponseDto> Create(
        CreateAddressRequestDto dto);

    Task<AddressResponseDto> Update(
        int id,
        UpdateAddressRequestDto dto);

    Task Delete(int id);
}