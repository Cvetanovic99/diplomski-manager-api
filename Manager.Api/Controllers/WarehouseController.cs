using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Api.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            this._warehouseService = warehouseService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWarehouses([FromQuery] PaginationParameters paginationParameters)
        {
            var warehouses = await _warehouseService.GetWarehousesAsync(paginationParameters);

            return Ok(warehouses);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();

            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWarehouse(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);

            return Ok(warehouse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWarehouse(CreateWarehouseDto createWarehouseDto)
        {
            var warehouse = await _warehouseService.CreateWarehouseAsync(createWarehouseDto);

            return Created("", warehouse);
        }


        [HttpPost]
        [Route("{warehouseId}/products/{productId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProductToWarehouse(int warehouseId, int productId, AddProductToWarehouse addProductToWarehouse)
        {
            addProductToWarehouse.WarehouseId = warehouseId;
            addProductToWarehouse.ProductId = productId;
  
            var addedProduct = await _warehouseService.AddProductToWarehouse(addProductToWarehouse);

            return Created("", addedProduct);
        }
        
        [HttpGet]
        [Route("{warehouseId}/products/{productId}/sn-codes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSnCodesForProduct(int warehouseId, int productId)
        {
            var snCodes = await _warehouseService.GetSnCodesForProduct(warehouseId, productId);

            return Ok(snCodes);
        }
        
        [HttpGet]
        [Route("{warehouseId}/products/{productId}/sn-codes-pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSnCodesForProductPagination(int warehouseId, int productId, [FromQuery]PaginationParameters paginationParameters)
        {
            var snCodes = await _warehouseService.GetSnCodesForProductPagination(warehouseId, productId, paginationParameters);

            return Ok(snCodes);
        }


        [HttpPut]
        [Route("update-product-state/{productId}/{warehouseId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductQuantityToWarehouse(int productId, int warehouseId, UpdateProductStateDto updateProductStateDto)
        {
            updateProductStateDto.ProductId = productId;
            updateProductStateDto.WarehouseId = warehouseId;

            var updatedProductState = await _warehouseService.UpdateProductStateToWarehouse(updateProductStateDto);

            return Ok(updatedProductState);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWarehouse(int id, UpdateWarehouseDto updateWarehouseDto)
        {
            updateWarehouseDto.Id = id;

            var warehouse = await _warehouseService.UpdateWarehouseAsync(updateWarehouseDto);

            return Ok(warehouse);
        }

        [HttpGet("{id}/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts(int id, [FromQuery] PaginationParameters paginationParameters)
        {
            var response = await _warehouseService.GetProductsAsync(id, paginationParameters);

            return Ok(response);
        }
        
        [HttpGet("{warehouseId}/products/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductWithoutSN(int warehouseId, int productId)
        {
            var response = await _warehouseService.GetProductStateWithoutSnAsync(warehouseId, productId);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            await _warehouseService.DeleteWarehouseAsync(id);

            return Ok();
        }

        [HttpGet("{id}/products-for-tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsForTasks(int id, [FromQuery] PaginationParameters paginationParameters)
        {
            var response = await _warehouseService.GetProductsForTasksAsync(id, paginationParameters);

            return Ok(response);
        }



        [HttpGet("{id}/non-existent-products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNonExistentProducts(int id, [FromQuery] PaginationParameters paginationParameters)
        {
            var response = await _warehouseService.GetNonExistentProductsAsync(id, paginationParameters);

            return Ok(response);
        }
    }
}
