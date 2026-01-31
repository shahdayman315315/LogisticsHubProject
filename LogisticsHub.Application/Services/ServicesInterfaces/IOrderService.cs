using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Helpers;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Services.ServicesInterfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<Order>> CreateOrderAsync(string userId, OrderDetailsDto orderDto);
        Task<ServiceResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(string userId);
        Task<ServiceResult<OrderDetailsDto>> GetOrderDetailsAsync(int orderId, string userId);
        Task<ServiceResult<bool>> CancelOrderAsync(int orderId, string userId);
        Task<ServiceResult<bool>> UpdateOrderAsync(int orderId,OrderStatus newStatus,string userId);
    }
}
