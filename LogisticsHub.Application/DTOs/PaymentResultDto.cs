using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class PaymentResultDto
    {
        public string SessionId { get; set; }=string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
