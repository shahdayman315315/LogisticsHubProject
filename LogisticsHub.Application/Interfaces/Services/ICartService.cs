using LogisticsHub.Application.Helpers;
using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<ServiceResult<Cart>> GetCartAsync();
        Task<ServiceResult<Cart>> AddToCart(int productId, int Quantity);
        Task<ServiceResult<bool>> UpdateQuantityAsync(int productId, int Quantity);
        Task<ServiceResult<bool>> RemoveItemAsync(int productId);
        Task<ServiceResult<bool>> ClearCartAsync();
        Task<ServiceResult<int>> GetTotalCountAsync();
    }
}
