using AutoMapper;
using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Application.Specifications;
using Manager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manager.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<ProductState> _productStateRepository;

        public WarehouseService(
            IMapper mapper, 
            IAsyncRepository<Warehouse> warehouseRepository,
            IAsyncRepository<Product> productRepository,
            IAsyncRepository<ProductState> productStateRepository)
        {
            this._mapper = mapper;
            this._warehouseRepository = warehouseRepository;
            this._productRepository = productRepository;
            this._productStateRepository = productStateRepository;
        }

        public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto)
        {
            var warehouse = _mapper.Map<Warehouse>(createWarehouseDto);

            await _warehouseRepository.AddAsync(warehouse);

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task DeleteWarehouseAsync(int warehouseId)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);

            if (warehouse is null)
            {
                throw new ApiException("Skladiste nije pronadjeno.", 404);
            }

            await _warehouseRepository.DeleteAsync(warehouse);
        }

        public async Task<IList<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetAsync();

            return _mapper.Map<IList<WarehouseDto>>(warehouses);
        }

        public async Task<PaginationResponse<ProductWarehouseDto>> GetProductsAsync(int warehouseId, PaginationParameters paginationParameters)
        {
            Expression<Func<Product, bool>> criteria = p => p.States.Any(state => state.Warehouse.Id == warehouseId && 
                            (state.Product.Model.Contains(paginationParameters.Keyword) || state.Product.Name.Contains(paginationParameters.Keyword)));//Da razmislimo da li da se pretrazuje po SN-u jer on trazi proizvod a tek posle u okviru proizvoda SN-ove.Jer moze da se desi da on trazi po SN-u i da mu prikaze taj proizvod a da quantity na state bude nula, to jest da pise da je kolicina proizvoda u tom skladistu nula a da ga ipak prikaze kad se unese taj SN.  

            var products = await _productRepository.GetBySpecAsync(
                new ProductByWarehouseIdSpecification(
                    warehouseId,
                    criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    orderBy: paginationParameters.OrderBy,
                    direction: paginationParameters.Direction));

            var count = await _productRepository.GetCountBySpecAsync(
                new ProductByWarehouseIdSpecification(warehouseId, criteria));

            return new PaginationResponse<ProductWarehouseDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProductWarehouseDto>>(products),
                Total = count
            };
        }

        public async Task<WarehouseDto> GetWarehouseByIdAsync(int warehouseId)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);

            if (warehouse is null)
            {
                throw new ApiException("Skladiste nije pronadjeno.", 404);
            }
            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<PaginationResponse<WarehouseDto>> GetWarehousesAsync(PaginationParameters paginationParameters)
        {
            Expression<Func<Warehouse, bool>> criteria = w => w.Name.Contains(paginationParameters.Keyword) || w.City.Contains(paginationParameters.Keyword);

            var warehouses = await _warehouseRepository.GetBySpecAsync(
               new WarehouseSpecification(
                   criteria: criteria,
                   start: paginationParameters.PageIndex * paginationParameters.PageSize,
                   take: paginationParameters.PageSize,
                   orderBy: paginationParameters.OrderBy,
                   direction: paginationParameters.Direction));

            var count = await _warehouseRepository.GetCountBySpecAsync(
                new WarehouseSpecification(criteria));

            return new PaginationResponse<WarehouseDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<WarehouseDto>>(warehouses),
                Total = count
            };
        }

        public async Task<WarehouseDto> UpdateWarehouseAsync(UpdateWarehouseDto updateWarehouseDto)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(updateWarehouseDto.Id);

            if (warehouse is null)
            {
                throw new ApiException("Skladiste nije pronadjeno.", 404);
            }

            _mapper.Map(updateWarehouseDto, warehouse);

            await _warehouseRepository.UpdateAsync(warehouse);

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<ProductDto> AddProductToWarehouse(AddProductToWarehouse addProductToWarehouse)
        {
            if (addProductToWarehouse.Quantity < 0)
                throw new ApiException("Kolicina ne sme da bude manja od nule.", 400);

            var productSate = await _productStateRepository.GetSingleBySpecAsync(new ProductStateSpecification(ps => ps.Warehouse.Id == addProductToWarehouse.WarehouseId && ps.Product.Id == addProductToWarehouse.ProductId));
            if (productSate is not null)
                throw new ApiException("Proizvod vec postoji u magacinu", 400);

            var warehouse = await _warehouseRepository.GetByIdAsync(addProductToWarehouse.WarehouseId);
            if (warehouse is null)
                throw new ApiException("Magacin nije pronadjen", 404);

            var product = await _productRepository.GetByIdAsync(addProductToWarehouse.ProductId);
            if (product is null)
                throw new ApiException("Proizvod nije pronadjen", 404);

            if (product.HasSN && CreateSnCodes(addProductToWarehouse.SNCodes).Count == 0)
                throw new ApiException("Za ovaj proizvod je potreban bar jedan SN", 400);

            if (!product.HasSN)
            {
                var productStateForCreation = new ProductState {Quantity = addProductToWarehouse.Quantity, Warehouse = warehouse, Product = product};
                await _productStateRepository.AddAsync(productStateForCreation);
                
                product.States.Add(productStateForCreation);
                return _mapper.Map<ProductDto>(product);
            }
            else
            {
                var snCodes = CreateSnCodes(addProductToWarehouse.SNCodes);
                List<ProductState> productStates = new List<ProductState>();

                foreach (var sn in snCodes)
                {
                    productStates.Add(new ProductState { Quantity = 1, Product = product, Warehouse = warehouse, SN = sn });    
                }

                await _productStateRepository.AddRangeAsync(productStates);
                product.States = productStates;
                
                return _mapper.Map<ProductDto>(product);
            }
        }

        public async Task<ProductDto> UpdateProductStateToWarehouse(UpdateProductStateDto updateProductStateDto)
        {
            var product = await _productRepository.GetByIdAsync(updateProductStateDto.ProductId);
            if (product is null)
                throw new ApiException("Proizvod cije stanje zelite da azurirate ne postoji.", 404);
            
            var warehouse = await _warehouseRepository.GetByIdAsync(updateProductStateDto.WarehouseId);
            if (warehouse is null)
                throw new ApiException("Skladiste cije stanje zelite da azurirate ne postoji.", 404);
            
            if (!product.HasSN)
            {
                var oldProductState = await _productStateRepository.GetSingleBySpecAsync(new ProductStateSpecification(
                    ps => ps.Warehouse == warehouse && ps.Product == product));
                if (oldProductState is null)
                    throw new ApiException("Stanje proizvoda nije pronadjeno.", 404);
                
                _mapper.Map(updateProductStateDto, oldProductState);
                await _productStateRepository.UpdateAsync(oldProductState);

                product.States.Add(oldProductState);
                return _mapper.Map<ProductDto>(product);
            }
            else
            {
                var productStatesFromDatabase = await _productStateRepository.GetBySpecAsync(
                    new ProductStateSpecification(
                        ps => ps.Product == product && ps.Warehouse == warehouse && ps.Quantity > 0,
                        start: null,
                        take: null,
                        orderBy: "Id",
                        direction: "ASC"));
                await _productStateRepository.DeleteRangeAsync(productStatesFromDatabase);

                var snCodes = CreateSnCodes(updateProductStateDto.SNCodes);

                List<ProductState> productStates = new List<ProductState>();
                foreach (var sn in snCodes)
                {
                    productStates.Add(new ProductState { Quantity = 1, Product = product, Warehouse = warehouse, SN = sn });    
                }
                await _productStateRepository.AddRangeAsync(productStates);

                product.States = productStates;
                return _mapper.Map<ProductDto>(product);
            }

        }

        public async Task<PaginationResponse<ProductDto>> GetNonExistentProductsAsync(int warehouseId, PaginationParameters paginationParameters)
        {
            Expression<Func<Product, bool>> criteria = p => p.States.All(s => s.Warehouse.Id != warehouseId && (s.Product.Name.Contains(paginationParameters.Keyword) || s.Product.Model.Contains(paginationParameters.Keyword)));//Sve proizvode koji se ne nalaze u magacinu.

            var products = await _productRepository.GetBySpecAsync(
               new ProductSpecification(
                   criteria: criteria,
                   start: paginationParameters.PageIndex * paginationParameters.PageSize,
                   take: paginationParameters.PageSize,
                   orderBy: paginationParameters.OrderBy,
                   direction: paginationParameters.Direction));

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

        public async Task<PaginationResponse<ProductWarehouseDto>> GetProductsForTasksAsync(int warehouseId, PaginationParameters paginationParameters)
        {
            Expression<Func<Product, bool>> criteria = p => p.States.Any(state => state.Warehouse.Id == warehouseId && state.Quantity > 0 && (state.SN.Contains(paginationParameters.Keyword) || state.Product.Name.Contains(paginationParameters.Keyword)));//Da vidimo da li da se izbrise i ovde pretrazivanje po SN jer on kad nadje proizvod i kad klikne na njega ope

            var products = await _productRepository.GetBySpecAsync(
                new ProductByWarehouseIdSpecification(
                    warehouseId,
                    criteria,
                    start: paginationParameters.PageIndex * paginationParameters.PageSize,
                    take: paginationParameters.PageSize,
                    orderBy: paginationParameters.OrderBy,
                    direction: paginationParameters.Direction));

            var count = await _productRepository.GetCountBySpecAsync(
                new ProductByWarehouseIdSpecification(warehouseId, criteria));

            return new PaginationResponse<ProductWarehouseDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProductWarehouseDto>>(products),
                Total = count
            };
        }

        public async Task<IList<ProductStateDto>> GetSnCodesForProduct(int warehouseId, int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new ApiException("Proizvod nije pronadjen", 404);
            
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse is null)
                throw new ApiException("Skladiste nije pronadjeno", 404);

            if (!product.HasSN)
                throw new ApiException("Ovo je proizvod koji nema SN kod", 400);

            var productStates = await _productStateRepository.GetBySpecAsync(new ProductStateSpecification(
                ps => ps.Product == product && ps.Warehouse == warehouse && ps.Quantity > 0,
                start: null,
                take: null,
                direction: "ASC",
                orderBy: "Id"));
            
            return _mapper.Map<IList<ProductStateDto>>(productStates);
        }

        public async Task<PaginationResponse<ProductStateDto>> GetSnCodesForProductPagination(int warehouseId, int productId, PaginationParameters paginationParameters)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new ApiException("Proizvod nije pronadjen", 404);
            
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);
            if (warehouse is null)
                throw new ApiException("Skladiste nije pronadjeno", 404);

            if (!product.HasSN)
                throw new ApiException("Ovo je proizvod koji nema SN kod", 400);

            Expression<Func<ProductState, bool>> criteria = ps => ps.Warehouse == warehouse && ps.Product == product && ps.Quantity > 0 &&
                                                                  ps.SN.Contains(paginationParameters.Keyword);
            
            var productStates = await _productStateRepository.GetBySpecAsync(new ProductStateSpecification(
                criteria: criteria,
                start: paginationParameters.PageIndex * paginationParameters.PageSize,
                take: paginationParameters.PageSize,
                direction: paginationParameters.Direction,
                orderBy: paginationParameters.OrderBy));
            
            var count = await _productStateRepository.GetCountBySpecAsync(
                new ProductStateSpecification(criteria));

            return new PaginationResponse<ProductStateDto>()
            {
                PageIndex = paginationParameters.PageIndex,
                PageSize = paginationParameters.PageSize,
                Items = _mapper.Map<IList<ProductStateDto>>(productStates),
                Total = count
            };
        }

        public async Task<ProductStateDto> GetProductStateWithoutSnAsync(int warehouseId, int productId)
        {
            var productState = await _productStateRepository.GetSingleBySpecAsync(
                new ProductStateSpecification(
                    ps => ps.ProductId == productId && ps.Warehouse.Id == warehouseId,
                    start: null,
                    take: null,
                    orderBy: "Id",
                    direction: "ASC"));
            
            return _mapper.Map<ProductStateDto>(productState);
        }

        public IList<string> CreateSnCodes(string codes)
        {
            var snCodes = codes.Split(",");
            snCodes = snCodes.Where(code => !string.IsNullOrWhiteSpace(code)).ToArray();
            
            for(int i=0; i<snCodes.Length; i++)
            {
                snCodes[i] = Regex.Replace(snCodes[i], @"\s+", "").Trim();
            }
            
            return snCodes;
        }
    }
}
