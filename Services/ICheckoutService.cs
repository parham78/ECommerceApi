public interface ICheckoutService
{
    Task<OrderResponseDto> Checkout(
        CheckoutRequestDto dto);
}