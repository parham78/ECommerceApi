using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/me/basket")]
[Authorize(Roles = AppRoles.Customer)]
public class MeBasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public MeBasketController(
        IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet]
    public async Task<ActionResult<BasketResponseDto>>
        GetMyBasket()
    {
        var basket =
            await _basketService.GetMyBasket();

        return Ok(basket);
    }

    [HttpPost("items")]
    public async Task<ActionResult<BasketResponseDto>>
        AddItem(
            AddBasketItemRequestDto dto)
    {
        var basket =
            await _basketService.AddItem(dto);

        return Ok(basket);
    }

    [HttpPatch("items/{itemId:int}")]
    public async Task<ActionResult<BasketResponseDto>>
        UpdateQuantity(
            int itemId,
            UpdateBasketItemRequestDto dto)
    {
        var basket =
            await _basketService.UpdateQuantity(
                itemId,
                dto);

        return Ok(basket);
    }

    [HttpDelete("items/{itemId:int}")]
    public async Task<ActionResult<BasketResponseDto>>
        RemoveItem(
            int itemId)
    {
        var basket =
            await _basketService.RemoveItem(
                itemId);

        return Ok(basket);
    }

    [HttpDelete("items")]
    public async Task<ActionResult<BasketResponseDto>>
        ClearBasket()
    {
        var basket =
            await _basketService.ClearBasket();

        return Ok(basket);
    }
}