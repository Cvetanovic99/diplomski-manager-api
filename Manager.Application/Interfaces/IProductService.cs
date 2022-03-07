using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Application.Dtos;

namespace Manager.Application.Interfaces
{
    public interface IProductService
    {
        Task<PaginationResponse<ProductDto>> GetProductsAsync(PaginationParameters paginationParameters);
        Task<IList<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int productId);
        Task<PaginationResponse<WarehouseProductDto>> GetWarehousesAsync(int productId, PaginationParameters paginationParameters);
        Task<float> GetSumOfProductQuantityAsync(int productId);
    }
}