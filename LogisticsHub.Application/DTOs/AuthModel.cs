using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class AuthModel
    {
        public string UserName { get; set; } = null!;

        public string Role {  get; set; }= null!;

        public string Message {  get; set; } = null!;

        public bool IsAuthenticated {  get; set; }

        public string Token { get; set; } = null!;
        public string RefreshToken {  get; set; } = null!;
        public DateTime ExpirationDate { get; set; }

    }
}
