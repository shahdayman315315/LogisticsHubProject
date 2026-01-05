using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public class CartItem
    {
        public int ProductId {  get; set; }
        public string ProductName {  get; set; }=string.Empty;
        public int Quantity {  get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
