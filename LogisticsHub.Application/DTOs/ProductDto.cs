using LogisticsHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class ProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        [Required]
        public int StoreId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
