public interface IBasketService
{
    Task<BasketResponseDto> GetMyBasket();

    Task<BasketResponseDto> AddItem(
        AddBasketItemRequestDto dto);

    Task<BasketResponseDto> UpdateQuantity(
        int itemId,
        UpdateBasketItemRequestDto dto);

    Task<BasketResponseDto> RemoveItem(
        int itemId);

    Task<BasketResponseDto> ClearBasket();
}