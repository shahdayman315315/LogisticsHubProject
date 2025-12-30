using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? Description {  get; set; }
        public int WalletId {  get; set; }
        public Wallet Wallet { get; set; } = null!;
        public Decimal Amount {  get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ExternalReferenceId {  get; set; }
    }
}
