using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Services;

public sealed class JwtSettings
{
    [MinLength(16)]
    public required string Secret { get; init; }

    [Required]
    [MinLength(1)]
    public required string Issuer { get; init; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<string> Audience { get; init; }
    
    [Required]
    [Range(0.16, 1440)]
    public required double AccessTokenExpirationMinutes { get; init; }
}