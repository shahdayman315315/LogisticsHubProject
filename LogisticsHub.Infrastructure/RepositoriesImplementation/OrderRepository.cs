using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Interfaces.Repositories;
using LogisticsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class OrderRepository:GenericRepository<Order>,IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context):base(context)
        {
            _context = context;
        }

    }
}
