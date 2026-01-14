using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ServiceResult<int>> CreateOrderAsync(string userId, string shippingAddress);
        Task<ServiceResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId);
        Task<ServiceResult<OrderDetailsDto>> GetOrderDetailsAsync(int orderId, string userId);
        Task<ServiceResult<bool>> CancelOrderAsync(int orderId, string userId);
        Task<ServiceResult<bool>> UpdateOrderAsync(int orderId,OrderStatus newStatus,string userId);
    }
}
