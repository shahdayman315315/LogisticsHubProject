using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMerchantRepository MerchantRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IStoreRepository StoreRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IWalletRepository WalletRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }

        IWithdrawalRequestsRepository WithdrawalRequestsRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        
        Task<int> CompleteAsync();        
    }
}
