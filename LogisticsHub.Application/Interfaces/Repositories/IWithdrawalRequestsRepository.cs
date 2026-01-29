using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Repositories
{
    public interface IWithdrawalRequestsRepository:IGenericRepository<WithDrawalRequest>
    {
        Task<IEnumerable<WithDrawalRequest>> GetRequestsWithDetailsAsync(Func<WithDrawalRequest, bool>? criteria = null, int? walletId = null);
    }
}
