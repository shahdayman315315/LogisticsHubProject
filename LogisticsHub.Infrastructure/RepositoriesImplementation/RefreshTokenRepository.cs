using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Data;
using LogisticsHub.Infrastructure.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.RepositoriesImplementation
{
    public class RefreshTokenRepository:GenericRepository<RefreshToken>,IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
