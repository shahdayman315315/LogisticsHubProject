using LogisticsHub.Domain.Interfaces.Repositories;
using LogisticsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private Lazy<IMerchantRepository> _merchantRepository;
        private Lazy<IStoreRepository> _storeRepository;
        private Lazy<IProductRepository> _productRepository;
        private Lazy<IOrderRepository> _orderRepository;
        private Lazy<IOrderItemRepository> _orderItemRepository;
        private Lazy<ITransactionRepository> _transactionRepository;
        private Lazy<IWalletRepository> _walletRepository;
        private Lazy<ICategoryRepository> _categoryRepository;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _categoryRepository = CreateRepository<ICategoryRepository, CategoryRepository>();
            _merchantRepository=CreateRepository<IMerchantRepository, MerchantRepository>();
            _orderItemRepository=CreateRepository<IOrderItemRepository, OrderItemRepository>(); 
            _productRepository=CreateRepository<IProductRepository, ProductRepository>();   
            _orderRepository=CreateRepository<IOrderRepository,OrderRepository>();
            _storeRepository=CreateRepository<IStoreRepository, StoreRepository>();
            _transactionRepository=CreateRepository<ITransactionRepository,TransactionRepository>();
            _walletRepository=CreateRepository<IWalletRepository, WalletRepository>();
        }

        private Lazy<T1> CreateRepository<T1, T2>() where T1 : class where T2:class,T1
        {
            return new Lazy<T1>(()=>(T1)Activator.CreateInstance(typeof(T2),_context)!);
        }

        public IMerchantRepository MerchantRepository => _merchantRepository.Value;

        public IOrderItemRepository OrderItemRepository => _orderItemRepository.Value;

        public IOrderRepository OrderRepository => _orderRepository.Value;

        public ICategoryRepository CategoryRepository => _categoryRepository.Value;

        public IProductRepository ProductRepository => _productRepository.Value;

        public IStoreRepository StoreRepository => _storeRepository.Value;

        public ITransactionRepository TransactionRepository => _transactionRepository.Value;

        public IWalletRepository WalletRepository => _walletRepository.Value;

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
