using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = null!;
        public Merchant? Merchant { get; set; }
        public Wallet? Wallet { get; set; }

        public ICollection<Order> Orders { get; set; }=new List<Order>();
    }
}
