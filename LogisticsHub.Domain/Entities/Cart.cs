using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Domain.Entities
{
    public class Cart
    {
        public List<CartItem> CartItems { get; set; }= new List<CartItem>();
        public int TotalQuantity => CartItems.Sum(i=>i.Quantity);
        public decimal TotalItemsPrice=>CartItems.Sum(i=>i.TotalPrice);

        public void AddItem(CartItem item)
        {
            var existItem = CartItems.FirstOrDefault(c => c.ProductId == item.ProductId);

            if (existItem is not null)
            {
                existItem.Quantity += item.Quantity;
            }

            CartItems.Add(item);
        }

        public void RemoveItem(CartItem item)
        {
            var existItem = CartItems.First(c => c.ProductId == item.ProductId);

            if (existItem is not null)
            {
                CartItems.Remove(item);
            }
        }

        public void UpdateItem(int productId,int newQuantity)
        {
            var existItem=CartItems.FirstOrDefault(c=>c.ProductId == productId);

            if (existItem is not null && newQuantity>0)
            {
                existItem.Quantity = newQuantity;
            }

            else if(newQuantity<=0)
            {
                CartItems.Remove(existItem);
            }
        }

        public void Clear()
        {
            CartItems.Clear();
        }
    }
}
