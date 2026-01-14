using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Decimal UnitPrice { get; set;}
        public int Quantity {  get; set;}
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
