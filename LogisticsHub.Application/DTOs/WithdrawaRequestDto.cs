using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class WithdrawaRequestDto
    {

        [Required]
        [Range(100,50000)]
        public decimal Amount {  get; set; }

        [Required]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string DestinationDetails { get; set; } = null!;
    }
}
