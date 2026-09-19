using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/me")]
[Authorize(Roles = AppRoles.Customer)]
public class MeController : ControllerBase
{
    private readonly IOrderService _orderService;

    public MeController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetMyOrders(
    int page = 1,
    int pageSize = 10)
    {
        var result =
            await _orderService.GetMyOrders(
                page,
                pageSize);

        return Ok(result);
    }

    [HttpGet("orders/{id}")]
    public async Task<IActionResult> GetMyOrderById(int id)
    {
        var result =
            await _orderService.GetMyOrderById(id);

        return Ok(result);
    }


    [HttpPatch("orders/{id}/cancel")]
    public async Task<IActionResult> CancelMyOrder(int id)
    {
        var order = await _orderService.CancelMyOrder(id);

        return Ok(order);
    }

}