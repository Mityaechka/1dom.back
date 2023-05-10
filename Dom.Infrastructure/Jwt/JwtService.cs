using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Taxiverk.Infrastructure.Jwt
{
    public class JwtService
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        public JwtService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        public string GenerateToken(JwtModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { 
                    new Claim(JwtOptions.ClaimsId, model.Id),
                    new Claim(JwtOptions.ClaimsType, ((int)model.Type).ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.TtlInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JwtModel ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Value.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                
                var userId = jwtToken.Claims.First(x => x.Type == JwtOptions.ClaimsId).Value;
                var userType = jwtToken.Claims.First(x => x.Type == JwtOptions.ClaimsType).Value;

                return new JwtModel
                {
                    Id = userId,
                    Type = Enum.Parse<JwtUserType>(userType)
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
