using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Application.Specifications;
using Manager.Core.Entities;

namespace Manager.Application.Services
{
    public class ProductService : IProductService
    {
        
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<ProductState> _productStateRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IAsyncRepository<Product> productRepository,
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<ProductState> productStateRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _productStateRepository = productStateRepository;
            _mapper = mapper;
        }
        
        public async Task<PaginationResponse<ProductDto>> GetProductsAsync(PaginationParameters paginationParameters)
        {
            Expression<Func<Product, bool>> criteria = p => p.States.Any(ps => ps.SN.Contains(paginationParameters.Keyword)) || p.Name.Contains(paginationParameters.Keyword);

            var products = await _productRepository.GetBySpecAsync(
                new ProductSpecification(
                    criteria: criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    orderBy: paginationParameters.OrderBy,
                    direction: paginationParameters.Direction)) ;

            var count = await _productRepository.GetCountBySpecAsync(
                new ProductSpecification(criteria));

            return new PaginationResponse<ProductDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProductDto>>(products),
                Total = count
            };
        }

        public async Task<IList<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAsync();

            return _mapper.Map<IList<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                throw new ApiException("Proizvod nije pronadjen", 404);
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);

            await _productRepository.AddAsync(product);
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(updateProductDto.Id);

            if (product is null)
            {
                throw new ApiException("Proizvod nije pronadjen", 404);
            }

            _mapper.Map(updateProductDto, product);

            await _productRepository.UpdateAsync(product);
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                throw new ApiException("Proizvod nije pronadjen", 404);
            }

            await _productRepository.DeleteAsync(product);
        }
        
        public async Task<PaginationResponse<WarehouseProductDto>> GetWarehousesAsync(int productId, PaginationParameters paginationParameters)
        {
            Expression<Func<Warehouse, bool>> criteria = w => w.States.Any(state => state.Product.Id == productId) &&
                                                              (w.Name.Contains(paginationParameters.Keyword) ||
                                                                w.City.Contains(paginationParameters.Keyword));

            var warehouses = await _warehouseRepository.GetBySpecAsync(
                new WarehouseByProductIdSpecification(
                    productId,
                    criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    orderBy: paginationParameters.OrderBy,
                    direction: paginationParameters.Direction));

            var count = await _warehouseRepository.GetCountBySpecAsync(
                new WarehouseByProductIdSpecification(productId, criteria));

            //Expression<Func<ProductState, float>> property = p => p.Quantity;
            //var totalSum = _productStateRepository.GetSumBySpecAsync(
            //    new ProductStateSpecification(ps => ps.Product.Id == productId,
            //    start: null,
            //    take: null,
            //    orderBy: "Id",
            //    direction: "ASC"), property);

            return new PaginationResponse<WarehouseProductDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<WarehouseProductDto>>(warehouses),
                Total = count
            };
        }

        public async Task<float> GetSumOfProductQuantityAsync(int productId)
        {
            var productStates = await _productStateRepository.GetBySpecAsync(
                new ProductStateSpecification(
                    ps => ps.Product.Id == productId,
                    start: null,
                    take: null,
                    orderBy: "Id",
                    direction: "ASC"));

            var sum = productStates.Select(ps => ps.Quantity).Sum();

            return sum;
        }
    }
}