using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuckleApp.Core.Enumerations;
using BuckleApp.Core.Entities;
using BuckleApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BuckleApp.Infrastructure.Security;

public class JwtGenerator : IJwtGenerator
{
    private readonly SymmetricSecurityKey _key;
    public JwtGenerator(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Token").Value));
    }
    public string CreateToken(User user, List<string> roles, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("organisationId", user.OrganisationId.ToString())
        };
        
        claims.AddRange(roles.Select(x => new Claim(ClaimsIdentity.DefaultRoleClaimType, x)));
        
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}