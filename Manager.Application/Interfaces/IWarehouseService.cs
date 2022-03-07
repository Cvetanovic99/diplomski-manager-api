using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<PaginationResponse<WarehouseDto>> GetWarehousesAsync(PaginationParameters paginationParameters);
        Task<IList<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseDto> GetWarehouseByIdAsync(int warehouseId);
        Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto warehouseDto);
        Task<WarehouseDto> UpdateWarehouseAsync(UpdateWarehouseDto updateWarehouseDto);
        Task DeleteWarehouseAsync(int warehouseId);
        Task<PaginationResponse<ProductWarehouseDto>> GetProductsAsync(int warehouseId, PaginationParameters paginationParameters);
        Task<ProductDto> AddProductToWarehouse(AddProductToWarehouse addProductToWarehouse);
        Task<ProductDto> UpdateProductStateToWarehouse(UpdateProductStateDto updateProductStateDto);
        Task<PaginationResponse<ProductDto>> GetNonExistentProductsAsync(int warehouseId, PaginationParameters paginationParameters);
        Task<PaginationResponse<ProductWarehouseDto>> GetProductsForTasksAsync(int warehouseId, PaginationParameters paginationParameters);
        Task<IList<ProductStateDto>> GetSnCodesForProduct(int warehouseId, int productId);
        Task<PaginationResponse<ProductStateDto>> GetSnCodesForProductPagination(int warehouseId, int productId, PaginationParameters paginationParameters);
        Task<ProductStateDto> GetProductStateWithoutSnAsync(int warehouseId, int productId);
        IList<string> CreateSnCodes(string codes);
    }
}
