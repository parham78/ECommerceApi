using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/me/checkout")]
[Authorize(Roles = AppRoles.Customer)]
public class MeCheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public MeCheckoutController(
        ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Checkout(
        CheckoutRequestDto dto)
    {
        var order = await _checkoutService.Checkout(dto);

        return Ok(order);
    }
}