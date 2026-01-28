using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class TransactionDto
    {
        public string? Description { get; set; }
        public Decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ExternalReferenceId { get; set; }
    }
}
