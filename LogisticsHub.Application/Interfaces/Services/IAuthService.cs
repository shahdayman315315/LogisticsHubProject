using LogisticsHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);

        Task<AuthModel> LoginAsync(LoginModel model);

        Task<AuthModel> RefreshTokenAsync(RefreshTokenModel model);

        Task ForgetPasswordAsync(ForgetPasswordModel model);

        Task<bool> ResetPasswordAsync(ResetPasswordModel model);
    }
}
