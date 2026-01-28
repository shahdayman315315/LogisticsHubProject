using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class WithdrawalRequestRepository :GenericRepository<WithDrawalRequest>:IWithdrawalRequestsRepository
    {
    }
}
