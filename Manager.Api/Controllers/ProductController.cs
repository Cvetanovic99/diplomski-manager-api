using System.Threading.Tasks;
using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Controllers
{
    //[Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery]PaginationParameters paginationParameters)
        {
            var response = await _productService.GetProductsAsync(paginationParameters);

            return Ok(response);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var product = await _productService.CreateProductAsync(createProductDto);

            return Created("", product);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            updateProductDto.Id = id;

            var product = await _productService.UpdateProductAsync(updateProductDto);

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);

            return Ok();
        }
        
        [HttpGet("{id}/warehouses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWarehouses(int id, [FromQuery]PaginationParameters paginationParameters)
        {
            var response = await _productService.GetWarehousesAsync(id, paginationParameters);

            return Ok(response);
        }

        [HttpGet("{productId}/sum-of-quantity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSumOfProductQuantity(int productId, [FromQuery] PaginationParameters paginationParameters)
        {
            var sum = await _productService.GetSumOfProductQuantityAsync(productId);

            return Ok(sum);
        }
    }
}