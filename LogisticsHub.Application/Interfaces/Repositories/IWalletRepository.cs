using LogisticsHub.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Repositories
{
    public interface IWalletRepository:IGenericRepository<Wallet>
    {
        Task<Wallet> GetWalletWithTransactionsAsync(String userId);

        Task<IEnumerable<Transaction>> GetWalletTransactionsAsync(int walletId,int pageNumber,int pageSize);
    }
}
