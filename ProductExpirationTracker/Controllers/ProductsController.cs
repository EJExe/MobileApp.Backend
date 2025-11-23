using Microsoft.AspNetCore.Mvc;
using ProductExpirationTracker.Application.DTOs;
using ProductExpirationTracker.Application.Interfaces;

namespace ProductExpirationTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] bool? archived, CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(archived, cancellationToken);
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(string id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto, CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // POST: api/products/batch
    [HttpPost("batch")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> CreateProducts(IEnumerable<CreateProductDto> dtos, CancellationToken cancellationToken)
    {
        var products = await _productService.CreateRangeAsync(dtos, cancellationToken);
        return Ok(products);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, UpdateProductDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _productService.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST: api/products/{id}/mark-as-used
    [HttpPost("{id}/mark-as-used")]
    public async Task<ActionResult<ProductDto>> MarkProductAsUsed(string id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.MarkAsUsedAsync(id, cancellationToken);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE: api/products/history
    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory(CancellationToken cancellationToken)
    {
        await _productService.ClearHistoryAsync(cancellationToken);
        return NoContent();
    }
}




