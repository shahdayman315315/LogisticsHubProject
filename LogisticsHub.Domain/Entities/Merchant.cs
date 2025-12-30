using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public class Merchant
    {
        public int Id { get; set; }
        public string? UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public string CommersialRegister { get; set; } = null!;
        public bool IsVerified {  get; set; }   
        public ICollection<Store> Stores { get; set; }=new List<Store>();
    }
}
