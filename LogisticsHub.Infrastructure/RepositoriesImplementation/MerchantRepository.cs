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
    public class MerchantRepository:GenericRepository<Merchant>,IMerchantRepository
    {
        private readonly AppDbContext _context;
        public MerchantRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
