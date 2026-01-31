using LogisticsHub.Infrastructure.Repositories.RepositoriesInterfaces;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Data;
using LogisticsHub.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.Repositories.RepositoriesImplementation
{
    public class RefreshTokenRepository:GenericRepository<RefreshToken>,IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }

        public async Task<RefreshToken> GetUserRefreshTokenAsync(string UserId,string refreshtoken)
        {
            
            return await _context.RefreshTokens.SingleOrDefaultAsync(r=>r.Token==refreshtoken&&r.UserId==UserId);
        }
    }
}
