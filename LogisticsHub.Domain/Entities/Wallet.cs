using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public decimal Balance {  get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; }=null!;
        public ICollection<Transaction> Transactions { get; set; }=new List<Transaction>();

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

    }
}
