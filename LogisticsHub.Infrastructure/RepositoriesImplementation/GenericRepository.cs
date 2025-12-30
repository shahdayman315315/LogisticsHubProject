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

        public void Delete(T entity)
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

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetFirstAsync(Expression<Func<T, bool>> criteria)
        {
            return await _dbSet.FirstOrDefaultAsync(criteria);
        }

        public  void Update(T entity)
        {
             _dbSet.Update(entity);
        }
    }
}
