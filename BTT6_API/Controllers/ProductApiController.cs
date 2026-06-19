using BTT6_API.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public class ProductApiController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductApiController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // 1. GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = await _productRepository.GetProductsAsync();
            return Ok(products);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    // 2. GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    // 3. POST: api/products
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        try
        {
            await _productRepository.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    // 4. PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        try
        {
            if (id != product.Id) return BadRequest();
            await _productRepository.UpdateProductAsync(product);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    // 5. DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}