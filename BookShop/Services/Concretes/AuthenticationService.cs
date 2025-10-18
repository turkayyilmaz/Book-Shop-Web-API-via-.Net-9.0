using AutoMapper;
using Entities.DTOs;
using Entities.Entities;
using Entities.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concretes
{
    public class AuthenticationService : IAuthenticationServices
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _user;
        private readonly IConfiguration _configuration;
        private User? _userFiled;
        public AuthenticationService(ILoggerService logger, IMapper mapper, UserManager<User> user, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _user = user;
            _configuration = configuration;
        }
        public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto)
        {
            var user = _mapper.Map<User>(userForRegistrationDto);
            var result = await _user.CreateAsync(user, userForRegistrationDto.Password);
            if (result.Succeeded && userForRegistrationDto.Roles != null && userForRegistrationDto.Roles.Any())
            {
                // Assign roles to the user, dikkat et role veya roles olabilir. Biz roles dedik çünkü birden fazla rol olabilir.
                await _user.AddToRolesAsync(user, userForRegistrationDto.Roles);
            }
            return result;
        }
        public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto)
        {
            _userFiled = await _user.FindByNameAsync(userForAuthenticationDto.UserName);
            var result = (_userFiled != null && await _user.CheckPasswordAsync(_userFiled, userForAuthenticationDto.Password));
            if (!result)
            {
                _logger.LogWarning($"{nameof(ValidateUserAsync)}: Authentication failed. Wrong user name or password.");
            }
            return result;
        }
        public async Task<TokenDto> CreateTokenAsync(bool populateExp)
        {
            var signinCredentials = GetSignInCredentials();
            var claims = await GetClaimsAsync();
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims);
            var refreshToken = GenerateRefreshToken();
            _userFiled.RefreshToken = refreshToken;
            if (populateExp)
                _userFiled.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _user.UpdateAsync(_userFiled);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }
        public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _user.FindByNameAsync(principal.Identity.Name);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequest();
            _userFiled = user;
            return await CreateTokenAsync(populateExp: false);
        }
        private SigningCredentials GetSignInCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings.GetSection("secretKey").Value);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaimsAsync()
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _userFiled.UserName)
                };
            var roles = await _user.GetRolesAsync(_userFiled);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("validIssuer").Value,
                audience: jwtSettings.GetSection("validAudience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires").Value)),
                signingCredentials: signinCredentials);
            return tokenOptions;
        }
        // refresh token oluşturma
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        // refresh token doğrulama
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true, // 
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("secretKey").Value)),
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
