using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class TransactionRepository:GenericRepository<Transaction>,ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
