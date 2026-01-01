using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>>? criteria=null);

        Task<T?> GetByIdAsync(object id);

        Task<T?> GetFirstAsync(Expression<Func<T, bool>> criteria);

        Task AddAsync(T entity);
        void Update(T entity);

        void Delete(T entity);

    }
}
