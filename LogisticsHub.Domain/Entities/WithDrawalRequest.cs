using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public  class WithDrawalRequest
    {
        public int Id { get; set; }

        public int WalletId {  get; set; }

        public Wallet Wallet {  get; set; }

        public decimal Amount { get; set; }

        public WithDrawalStatus Status { get; set; }

        public string PaymentMethod {  get; set; }=string.Empty;

        public string DestinationDetails {  get; set; }= string.Empty;

        public string AdminComment {  get; set; }=string.Empty;
    }
}
