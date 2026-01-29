using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class WithdrawalRequestRepository :GenericRepository<WithDrawalRequest>,IWithdrawalRequestsRepository
    {
        private readonly AppDbContext _context;

        public WithdrawalRequestRepository(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WithDrawalRequest>> GetRequestsWithDetailsAsync(Expression<Func<WithDrawalRequest,bool>>? criteria=null, int? walletId = null)
        {
            IQueryable<WithDrawalRequest> requests;

            requests =  _context.WithDrawalRequests.Include(r => r.Wallet).ThenInclude(w => w.User).AsNoTracking();

            if (walletId.HasValue)
            {
                requests=requests.Where(r => r.WalletId == walletId);
            }

            if(criteria is not null)
            {
                requests= requests.Where(criteria);
            }

            return await requests.ToListAsync();
        }
    }
}
