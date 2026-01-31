using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LogisticsHub.Infrastructure.Repositories.RepositoriesImplementation
{
    public class OrderRepository:GenericRepository<Order>,IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderWithDetailsAsync(int orderId, string userId)
        {
            return await _context.Orders.Include(o=>o.OrderItems).ThenInclude(oi=>oi.Product).FirstOrDefaultAsync(o=>o.Id==orderId);
        }
    }
}
