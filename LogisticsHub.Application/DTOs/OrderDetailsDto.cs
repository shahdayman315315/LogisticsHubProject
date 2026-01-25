using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public string ShippingAddress {  get; set; }=string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Processing;
        public decimal TotalAmount {  get; set; }
        public List<OrderItemDto> OrderItems { get; set; }=new List<OrderItemDto>();
        
    }
}
