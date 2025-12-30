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
    public class WalletRepository:GenericRepository<Wallet>,IWalletRepository
    {
        private readonly AppDbContext _context;
        public WalletRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
