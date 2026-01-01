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
        public AuthService(UserManager<ApplicationUser> userManager,IOptions<JWT> jwt,AppDbContext context,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _jwt = jwt.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

               await _unitOfWork.MerchantRepository.AddAsync(merchant);
               await _unitOfWork.CompleteAsync();
            }

            var jwtToken = await GenerateJWTToken(user);

            var refreshToken = new RefreshToken()
            {
                Token = new Guid().ToString(),
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
                audience: _jwt.Audiance,
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
                Token = new Guid().ToString(),
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
            authModel.Message = "Login Successfully";

            return authModel;

        }

        
    }
}
