using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    internal class Merchant
    {
        public int Id { get; set; }
        public string? UserId {  get; set; }
        public string CommersialRegister { get; set; } = null!;
        public bool IsVerified {  get; set; }   
        public int WalletId {  get; set; }
        public Wallet Wallet { get; set; } = null!;
        public ICollection<Store> Stores { get; set; }=new List<Store>();
    }
}
