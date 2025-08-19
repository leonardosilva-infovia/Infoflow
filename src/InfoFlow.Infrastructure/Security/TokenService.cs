using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Shared.Jwt;
using Microsoft.Extensions.Options;             // << ADICIONE ESTE USING
using Microsoft.IdentityModel.Tokens;

namespace InfoFlow.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwt;

    // Injete IOptions<JwtOptions> (Options Pattern)
    public TokenService(IOptions<JwtOptions> options)
    {
        _jwt = options?.Value ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(_jwt.Key) || _jwt.Key.Length < 32)
            throw new InvalidOperationException("JwtOptions.Key deve ter pelo menos 32 caracteres.");
    }

    public GeneratedToken GenerateTokens(
        Guid userId,
        string userName,
        string? fullName,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        IEnumerable<Claim>? extraClaims = null)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new("name", fullName ?? userName),
        };

        foreach (var r in roles ?? Enumerable.Empty<string>())
            claims.Add(new Claim(ClaimTypes.Role, r));

        foreach (var p in permissions ?? Enumerable.Empty<string>())
            claims.Add(new Claim("perm", p));

        if (extraClaims is not null)
            claims.AddRange(extraClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_jwt.AccessTokenLifetimeMinutes),
            signingCredentials: creds
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(jwt);

        return new GeneratedToken(
            AccessToken: accessToken,
            ExpiresAt: jwt.ValidTo,
            RefreshToken: string.Empty, // refresh real via IRefreshTokenService
            RefreshExpiresAt: now.AddDays(_jwt.RefreshTokenLifetimeDays));
    }
}