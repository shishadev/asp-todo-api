using Domain.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public sealed class TokenGenerator : ITokenGenerator
{
    private readonly IOptionsSnapshot<JwtSettings> _settingsOptions;

    public TokenGenerator(IOptionsSnapshot<JwtSettings> settingsOptions)
    {
        _settingsOptions = settingsOptions;
    }

    public string Generate(string userId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("userId", userId)
        };

        return CreateToken(claims, _settingsOptions.Value.Audience);
    }

    public string GenerateRegistrationToken(string clientId, string clientName)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, clientId),
            new Claim("clientId", clientId),
            new Claim("clientName", clientName)
        };

        return CreateToken(claims, _settingsOptions.Value.Audience);
    }
    
    // public string GenerateClientServiceToken(string clientId, string clientName)
    // {
    //     var claims = new[]
    //     {
    //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //         new Claim(JwtRegisteredClaimNames.Sub, clientId),
    //         new Claim("clientId", clientId),
    //         new Claim("clientName", clientName)
    //     };
    //
    //     return CreateToken(claims, _settings.Audience);
    // }

    private SecurityTokenDescriptor CreateTokenDescriptor(IEnumerable<Claim> claims, IReadOnlyList<string> audience)
    {
        var settings = _settingsOptions.Value;
        var key = settings.Secret;
        var keyByteArray = Encoding.UTF8.GetBytes(key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes),
            Issuer = settings.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyByteArray),
            SecurityAlgorithms.HmacSha256Signature)
        };

        if (audience.Count > 0)
        {
            tokenDescriptor.Audience = string.Join(", ", audience);
        }

        return tokenDescriptor;
    }

    private string CreateToken(IEnumerable<Claim> claims, IReadOnlyList<string> audience)
    {
        var tokenDescriptor = CreateTokenDescriptor(claims, audience);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}