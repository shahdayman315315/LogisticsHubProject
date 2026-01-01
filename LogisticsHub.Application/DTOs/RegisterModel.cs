using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.DTOs
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required,EmailAddress]
        public string Email { get; set; } = null!;

        [Required,DataType(DataType.Password)]
        public string Password {  get; set; } = null!;

        [Required,Compare("Password")]
        public string PasswordConfirmed { get; set; } = null!;

        [Required]
        public string Role {  get; set; } = null!;

        public string? CommersialRegister {  get; set; }


    }
}
