using LogisticsHub.Domain.Entities;
using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class WalletRepository:GenericRepository<Wallet>,IWalletRepository
    {
        private readonly AppDbContext _context;
        public WalletRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }

        public async Task<Wallet> GetWalletWithTransactionsAsync(string userId)
        {
            return await _context.Wallets.Include(w => w.Transactions).FirstOrDefaultAsync(w=>w.UserId==userId);
        }
    }
}
