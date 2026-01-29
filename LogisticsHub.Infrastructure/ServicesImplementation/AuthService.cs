using AutoMapper;
using LogisticsHub.Application.DTOs;
using LogisticsHub.Application.Interfaces.Repositories;
using LogisticsHub.Application.Interfaces.Services;
using LogisticsHub.Domain.Entities;
using LogisticsHub.Domain.Helpers;
using LogisticsHub.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.ServicesImplementation
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        public AuthService(UserManager<ApplicationUser> userManager,IOptions<JWT> jwt,AppDbContext context,
            IMapper mapper, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            var existingUser=await _userManager.FindByEmailAsync(model.Email);
            if(existingUser is not null)
            {
                return new AuthModel()
                {
                    Message = "Email already exists."
                };
            }
            var user=_mapper.Map<ApplicationUser>(model);

            var result=await _userManager.CreateAsync(user,model.Password);

            if (!result.Succeeded)
            {
                return new AuthModel()
                {
                    Message = String.Join(',', result.Errors.Select(e => e.Description))
                };
            }

            if (model.Role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            else
            {
                await _userManager.AddToRoleAsync(user,"Merchant");

                var merchant = new Merchant()
                {
                    UserId = user.Id,
                    User = user,
                    CommersialRegister = model.CommersialRegister!,
                    IsVerified = true
                };

                var wallet = new Wallet
                {
                    User = user,
                    Balance = 0
                };

               await _unitOfWork.MerchantRepository.AddAsync(merchant);
               await _unitOfWork.WalletRepository.AddAsync(wallet);
               await _unitOfWork.CompleteAsync();
            }

            var jwtToken = await GenerateJWTToken(user);

            var refreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();

            return new AuthModel()
            {
                UserName = user.UserName!,
                IsAuthenticated = true,
                Role = model.Role,
                Message = "Registered Successfully",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken=refreshToken.Token,
                ExpirationDate = jwtToken.ValidTo
            };
        }

        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles=await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(ClaimTypes.NameIdentifier,user.Id)

            }.Union(userClaims)
             .Union(roleClaims);

            var symmeticSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials= new SigningCredentials(symmeticSecurityKey,SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken
                (
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: jwtClaims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials
                );
            
            return jwtSecurityToken;
        }

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var authModel=new AuthModel();
            var user=await _userManager.FindByEmailAsync(model.Email);
            
            if(user is null|| !await _userManager.CheckPasswordAsync(user,model.Password))
            {
                authModel.Message = "Incorrect Email or Password.";
            }

            var jwtToken = await GenerateJWTToken(user!);

            var refreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                UserId = user!.Id
            };

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();

            authModel.IsAuthenticated=true;
            authModel.ExpirationDate = jwtToken.ValidTo;
            authModel.RefreshToken=refreshToken.Token;
            authModel.Token=new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var roles= await _userManager.GetRolesAsync(user!);
            authModel.Role = roles.FirstOrDefault()!;
            authModel.UserName = user!.UserName!;
            authModel.Message = "Logged in Successfully";

            return authModel;

        }

        public async Task<AuthModel> RefreshTokenAsync(RefreshTokenModel model)
        {
            var authmodel = new AuthModel();
            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
            var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                authmodel.Message = "Invalid Token";
                return authmodel;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                authmodel.Message = "User Not Found";
                return authmodel;
            }

            var storedRefreshToken =await _unitOfWork.RefreshTokenRepository.GetUserRefreshTokenAsync(userId,model.Refreshtoken);
            if (storedRefreshToken is null || storedRefreshToken.ExpirationDate < DateTime.UtcNow
                || storedRefreshToken.IsUsed || storedRefreshToken.IRevoked)
            {
                authmodel.Message = "Invalid RefreshToken";
                return authmodel;
            }
            storedRefreshToken.IsUsed = true;

            var jwtToken = await GenerateJWTToken(user);

            var RefreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                IRevoked = false
            };

            await _unitOfWork.RefreshTokenRepository.AddAsync(RefreshToken);
            await _unitOfWork.CompleteAsync();

            authmodel.IsAuthenticated= true;
            authmodel.Token=new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authmodel.RefreshToken = RefreshToken.Token;
            authmodel.ExpirationDate = jwtToken.ValidTo;
            authmodel.UserName = user.UserName!;

            return authmodel;
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer=true,
                ValidateAudience=true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, 
                ValidIssuer = _jwt.Issuer,
                ValidAudience = _jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.Key))
            };

            var tokenHandler=new JwtSecurityTokenHandler();

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken=securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; // 512 bits

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public async Task ForgetPasswordAsync(ForgetPasswordModel model)
        {
            var user=await _userManager.FindByEmailAsync(model.Email);

            if(user is null)
            {
                return;
            }

            var resetToken=await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"https://localhost:7002/api/auth/resetpassword?email={model.Email}&token={Uri.EscapeDataString(resetToken)}";
        
            await _emailService.SendEmailAsync(model.Email, "Reset Your Password",
            $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>"
            );
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user is null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            return result.Succeeded;
        }
    }
}
