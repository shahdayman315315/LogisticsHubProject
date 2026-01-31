using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Application.Services.ServicesInterfaces;
using LogisticsHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Services.ServicesImplementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private const string ProductsCachKey = "ProductsKey";
        public ProductService(IMemoryCache memoryCache,IUnitOfWork unitOfWork,IMapper mapper)
        {
            _mapper = mapper;
            _memoryCache = memoryCache;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult<Product>> AddProductAsync(ProductAddDto productdto)
        {
            var existProduct=await _unitOfWork.ProductRepository.GetFirstAsync(
                p=>p.Name.Trim().ToLower()==productdto.Name.Trim().ToLower());

            if (existProduct is not null) 
            {
                return ServiceResult<Product>.Failure("Product already exists.");

            }
            var existStore= await _unitOfWork.StoreRepository.GetByIdAsync(productdto.StoreId);

            if(existStore is null)
            {
                return ServiceResult<Product>.Failure("Store doesn't exist.");
            }

            var existCategory=await _unitOfWork.CategoryRepository.GetByIdAsync(productdto.CategoryId);

            if(existCategory is null)
            {
                return ServiceResult<Product>.Failure("Category doesn't exist.");
            }

            var product = _mapper.Map<Product>(productdto);
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            _memoryCache.Remove(ProductsCachKey);

            return ServiceResult<Product>.Success(product);

        }

        public async Task<ServiceResult<bool>> DeleteProductAsync(int productId)
        {
            var existProduct=await _unitOfWork.ProductRepository.GetByIdAsync(productId);

            if(existProduct is null)
            {
                return ServiceResult<bool>.Failure("Product doesn't exist.",404);
            }

             _unitOfWork.ProductRepository.Delete(existProduct);
             await _unitOfWork.CompleteAsync();

             _memoryCache.Remove(ProductsCachKey);


            return ServiceResult<bool>.Success(true,"Product deleted Successfully");

        }

        public async Task<ServiceResult<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {

            if(!_memoryCache.TryGetValue(ProductsCachKey, out IEnumerable<ProductDto> products))
            {
                var existProducts = await _unitOfWork.ProductRepository.GetAllAsync();
                 products=_mapper.Map<IEnumerable<ProductDto>>(existProducts);

                var cachOpttions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(20))
                    .SetPriority(CacheItemPriority.Normal);

                _memoryCache.Set(ProductsCachKey, products,cachOpttions);

            }

            return ServiceResult<IEnumerable<ProductDto>>.Success(products!);
        }

        public async Task<ServiceResult<ProductDto>> UpdateProductAsync(ProductDto productdto)
        {
            try
            {
                var existProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productdto.Id);

                if (existProduct is null)
                {
                    return ServiceResult<ProductDto>.Failure("Product doesn't exist.",404);
                }

                var existStore = await _unitOfWork.StoreRepository.GetByIdAsync(productdto.StoreId);

                if (existStore is null)
                {
                    return ServiceResult<ProductDto>.Failure("Store doesn't exist");
                }

                var existCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(productdto.CategoryId);

                if (existCategory is null)
                {
                    return ServiceResult<ProductDto>.Failure("Category doesn't exist");
                }

                _mapper.Map(productdto, existProduct);
                await _unitOfWork.CompleteAsync();

                _memoryCache.Remove(ProductsCachKey);

                return ServiceResult<ProductDto>.Success(productdto);
            }

            catch (DbUpdateConcurrencyException ex)
            {
                return ServiceResult<ProductDto>.Failure($"{ex.Message}",409);
            }

            catch(Exception ex)
            {
                return ServiceResult<ProductDto>.Failure($"An Error Occured : {ex.Message} ");
            }
        }

        public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var existProduct =await _unitOfWork.ProductRepository.GetByIdAsync(id);

            if (existProduct is null)
            {
                return ServiceResult<ProductDto>.Failure("Product doesn't exist.", 404);
            }

            var product=_mapper.Map<ProductDto>(existProduct);

            return ServiceResult<ProductDto>.Success(product);
        }

        
        
    }
}
