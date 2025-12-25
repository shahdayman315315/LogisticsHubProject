using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    internal class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity{get ; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public int StoreId {  get; set; }
        public Store? Store { get; set; }
        public int CategoryId {  get; set; }
        public Category? Category { get; set; }
    }
}
