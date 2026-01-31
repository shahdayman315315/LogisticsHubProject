using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces
{
    public interface IWithdrawalRequestsRepository:IGenericRepository<WithDrawalRequest>
    {
        Task<IEnumerable<WithDrawalRequest>> GetRequestsWithDetailsAsync(Expression<Func<WithDrawalRequest, bool>>? criteria = null, int? walletId = null);
    }
}
