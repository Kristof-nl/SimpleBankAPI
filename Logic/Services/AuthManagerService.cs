using Data.DataObjects;
using Logic.DataTransferObjects.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{

    public interface IAuthManagerService
    {
        Task<bool> ValidateUser(LoginUserDto loginUserDto);
        Task<string> CreateToken();
    }


    public class AuthManagerService : IAuthManagerService
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;

        public AuthManagerService(UserManager<ApiUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateToken()
        {
            var signingCredential = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredential, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("KEY2");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateUser(LoginUserDto userDto)
        {
            _user = await _userManager.FindByNameAsync(userDto.Email);
            return (_user != null && await _userManager.CheckPasswordAsync(_user, userDto.Password));
        }
    }
}
