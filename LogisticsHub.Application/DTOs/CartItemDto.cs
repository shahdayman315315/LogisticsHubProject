using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class CartItemDto
    {
        [Required]
        public int ProductId { get; set;}

        [Required]
        public int Quantity {  get; set;}
    }
}
