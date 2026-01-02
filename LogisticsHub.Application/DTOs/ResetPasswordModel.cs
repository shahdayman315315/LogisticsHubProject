using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class ResetPasswordModel
    {
        [Required,EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;

        [Required,DataType(DataType.Password)]
        public string NewPassword { get; set; }=null!;

        [Required,Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
