using LogisticsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    internal class Order
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string CustomerId {  get; set; }
        public ApplicationUser User { get; set; } = null!;
        public OrderStatus? Status { get; set; } = OrderStatus.Pending;

        public string ShippingAddress { get; set; } = null!;
        public decimal TotalAmount {  get; set; }
        public decimal PlatformCommision {  get; set; }

        public string? StripeSessionId {  get; set; }

        public  DateTime CreatedAt {  get; set; }= DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public ICollection<OrderItem> OrderItems = new List<OrderItem>();

    }
}
