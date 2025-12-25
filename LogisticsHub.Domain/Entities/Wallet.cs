using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    internal class Wallet
    {
        public int Id { get; set; }
        public decimal Balance {  get; set; }
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
    }
}
