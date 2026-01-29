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

        public async Task<IEnumerable<Transaction>> GetWalletTransactionsAsync(int walletId, int pageNumber, int pageSize)
        {
            return await _context.Transactions.Where(t => t.WalletId == walletId).OrderByDescending(t=>t.CreatedAt)
                .Skip((pageNumber-1)*pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        }

        public async Task<Wallet> GetWalletWithTransactionsAsync(string userId)
        {
            return await _context.Wallets.Include(w => w.Transactions.Take(5)).FirstOrDefaultAsync(w=>w.UserId==userId);
        }
    }
}
