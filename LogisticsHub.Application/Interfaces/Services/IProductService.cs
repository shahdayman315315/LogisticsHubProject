using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ServiceResult<Product>> AddProductAsync(ProductAddDto product);
        Task<ServiceResult<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<ServiceResult<ProductDto>> UpdateProductAsync(ProductDto product);
        Task<ServiceResult<bool>> DeleteProductAsync(int productId);
        Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id);
    }
}
