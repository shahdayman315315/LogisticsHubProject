using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.Repositories.RepositoriesImplementation
{
    public class StoreRepository:GenericRepository<Store>,IStoreRepository
    {
        private readonly AppDbContext _context;
        public StoreRepository(AppDbContext context):base(context)
        {
            _context = context;
        }
    }
}
