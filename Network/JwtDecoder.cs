using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Network;

public class JwtDecoder(string jwk)
{
    private readonly TokenValidationParameters _jwtValidateParameter = new()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwk)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    
    public IEnumerable<Claim> Decode(byte[] payload)
        => _jwtSecurityTokenHandler
            .ValidateToken(
                Encoding.Default.GetString(payload), _jwtValidateParameter, out _)
            .Claims;
}