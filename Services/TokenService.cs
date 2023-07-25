using MediaApi.Settings;
using MediaApi.Models;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace MediaApi.Services
{
    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AuthSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static int DecodeToken(string token){
            var tokenHandler = new JwtSecurityTokenHandler();

            // Prevent invalid Token Error, like "Token: hello World"
            if(tokenHandler.CanReadToken(token) == false)
                return -1;
            
            JwtSecurityToken jsonToken;

            // Prevent another Token Error
            try{
                jsonToken =  tokenHandler.ReadJwtToken(token);
            }
            catch{
                return -1;
            }
            
            // jti -> Json Token Identifier 
            string jti = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name").Value;
        
            return Convert.ToInt32(jti);
        }
    }

}