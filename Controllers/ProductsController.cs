using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    string? search = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    bool? inStock = null,
    string? sortBy = null,
    string? sortDirection = null,
    int page = 1,
    int pageSize = 10)
    {
        var products = await _productService.GetAll(
            search,
            minPrice,
            maxPrice,
            inStock,
            sortBy,
            sortDirection,
            page,
            pageSize);

        return Ok(products);
    }
    [HttpGet("admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAllForAdmin(
    bool? isActive = null,
    int page = 1,
    int pageSize = 10)
    {
        var products = await _productService.GetAllForAdmin(
            isActive,
            page,
            pageSize);

        return Ok(products);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetById(id);

        return Ok(product);
    }
    [HttpGet("admin/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetByIdForAdmin(int id)
    {
        var product = await _productService.GetByIdForAdmin(id);

        return Ok(product);
    }






    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Add(
        CreateProductRequestDto dto)
    {
        var product = await _productService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }


    [HttpPut("{id}/stock")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UpdateStock(
    int id,
    [FromBody] UpdateStockRequestDto dto)
    {
        var product =
            await _productService.UpdateStock(id, dto);

        return Ok(product);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.Delete(id);

        return NoContent();
    }
    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UpdateProduct(
    int id,
    UpdateProductRequestDto dto)
    {
        var product = await _productService.Update(id, dto);

        return Ok(product);
    }
}