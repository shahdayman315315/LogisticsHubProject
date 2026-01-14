using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Repositories
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        Task<Order> GetOrderWithDetailsAsync(int orderId, string userId);
    }
}
