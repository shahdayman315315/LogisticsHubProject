using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var result=await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var result= await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            var result=await _authService.RefreshTokenAsync(model);

            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            return Ok(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }
            
            await _authService.ForgetPasswordAsync(model);

            return Ok("If the email exists, a reset link has been sent.");
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var result=await _authService.ResetPasswordAsync(model);

            if (!result)
            {
                return Unauthorized("Invalid token or email");
            }

            return Ok("Password has been reset successfully");
        }
    }
}
