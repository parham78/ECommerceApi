using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
    int page = 1,
    int pageSize = 10)
    {
        var orders = await _orderService.GetAll(
            page,
            pageSize);

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetById(id);

        return Ok(order);
    }






    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(
    int id,
    ChangeOrderStatusRequestDto dto)
    {
        var order = await _orderService.ChangeStatus(id, dto);

        return Ok(order);
    }
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _orderService.CancelOrder(id);

        return Ok(order);
    }
}