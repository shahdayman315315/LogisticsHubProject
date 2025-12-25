using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    internal class Store
    {
        public int Id {  get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? LogoUrl {  get; set; }
        public decimal CommisionRate {  get; set; }
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
        public ICollection<Product> Products { get; set; }=new List<Product>();

    }
}
