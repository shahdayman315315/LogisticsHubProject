using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class RefreshTokenModel
    {
        public string AccessToken { get; set; } = null!;
        public string Refreshtoken { get; set; } = null!;
    }
}
