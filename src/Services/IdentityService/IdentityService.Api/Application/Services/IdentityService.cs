using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public Task<LoginResponseModel> Login(LoginRequestModel requestModel)
        {
            var claims = new Claim[] { 
            new Claim(ClaimTypes.NameIdentifier, requestModel.UserName),
            new Claim(ClaimTypes.Name, $"Burak Durmuş")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Secretkeyforburakdurmusauthentication"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(10);

            var token = new JwtSecurityToken(claims: claims, signingCredentials: creds, expires: expiry, notBefore: DateTime.Now);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            LoginResponseModel responseModel = new LoginResponseModel
            {
                UserName = requestModel.UserName,
                UserToken = encodedJwt
            };
            return Task.FromResult(responseModel);
        }
    }
}
