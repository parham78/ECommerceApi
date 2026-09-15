using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/me/addresses")]
[Authorize(Roles = AppRoles.Customer)]
public class MeAddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public MeAddressesController(
        IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AddressResponseDto>>>
        GetMyAddresses()
    {
        var addresses =
            await _addressService.GetMyAddresses();

        return Ok(addresses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AddressResponseDto>>
        GetMyAddressById(int id)
    {
        var address =
            await _addressService.GetMyAddressById(id);

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<AddressResponseDto>>
        Create(CreateAddressRequestDto dto)
    {
        var address =
            await _addressService.Create(dto);

        return CreatedAtAction(
            nameof(GetMyAddressById),
            new { id = address.Id },
            address);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AddressResponseDto>>
        Update(
            int id,
            UpdateAddressRequestDto dto)
    {
        var address =
            await _addressService.Update(id, dto);

        return Ok(address);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _addressService.Delete(id);

        return NoContent();
    }
}