using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogisticsHub.Application.Interfaces.Repositories;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class CategoryRepository:GenericRepository<Category>,ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context):base(context)
        {
            _context = context;
        }
    }
}
