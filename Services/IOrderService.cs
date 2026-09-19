public interface IOrderService
{
    Task<PagedResultDto<OrderResponseDto>> GetAll(
    int page,
    int pageSize);
    Task<PagedResultDto<OrderResponseDto>> GetMyOrders(
    int page,
    int pageSize);
    Task<OrderResponseDto> GetMyOrderById(int id);


    Task<OrderResponseDto> GetById(int id);

    Task<OrderResponseDto> ChangeStatus(
        int id,
        ChangeOrderStatusRequestDto dto);
    Task<OrderResponseDto> CancelOrder(int id);
    Task<OrderResponseDto> CancelMyOrder(int id);
}