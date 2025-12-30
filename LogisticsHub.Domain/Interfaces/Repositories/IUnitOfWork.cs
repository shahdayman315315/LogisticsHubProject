using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Interfaces.Repositories
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
        

        Task<int> CompleteAsync();        
    }
}
