using LogisticsHub.Domain.Interfaces.Repositories;
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
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext context)
        { 
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null)
        {
            if(criteria is null)
            {
                return await _dbSet.AsNoTracking().ToListAsync();
            }

            return await  _dbSet.Where(criteria).AsNoTracking().ToListAsync();
        }

        public Task<T?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetFirstAsync(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
