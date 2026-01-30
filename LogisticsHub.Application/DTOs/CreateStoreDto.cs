using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public  class CreateStoreDto
    {
        [Required,StringLength(50,MinimumLength =5)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }

        [Required]
        public decimal CommissionRate { get; set; }
        public int? MerchantId { get; set; } 
    }
}
