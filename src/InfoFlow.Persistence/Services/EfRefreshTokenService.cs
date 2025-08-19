using System.Security.Cryptography;
using InfoFlow.Application.Security.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace InfoFlow.Persistence.Services;

using InfoFlow.Domain.Security.Entities;   // sua entidade
using InfoFlow.Persistence.DbContexts;     // seu DbContext

public class EfRefreshTokenService : IRefreshTokenService
{
    private readonly SecurityDbContext _db;

    public EfRefreshTokenService(SecurityDbContext db) => _db = db;

    public async Task<string> IssueAsync(Guid userId, TimeSpan ttl, string? device = null, string? ip = null)
    {
        var token = GenerateSecureToken();
        while (await _db.Set<RefreshToken>().AnyAsync(t => t.Token == token))
            token = GenerateSecureToken();

        var now = DateTime.UtcNow;

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = now,
            CreatedByIp = ip,
            ExpiresAt = now.Add(ttl),
            RevokedAt = null,
            ReplacedByToken = null
        };

        _db.Set<RefreshToken>().Add(entity);
        await _db.SaveChangesAsync();
        return token;
    }

    public async Task<ValidatedRefresh> ValidateAsync(string refreshToken)
    {
        var e = await _db.Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (e is null)
            throw new UnauthorizedAccessException("Refresh token inválido.");

        if (!e.IsActive)
        {
            if (e.RevokedAt is not null)
                throw new UnauthorizedAccessException("Refresh token revogado.");
            if (DateTime.UtcNow > e.ExpiresAt)
                throw new UnauthorizedAccessException("Refresh token expirado.");

            throw new UnauthorizedAccessException("Refresh token inativo.");
        }

        return new ValidatedRefresh(
            UserId: e.UserId,
            ExpiresAt: e.ExpiresAt,
            Device: null,
            Ip: e.CreatedByIp
        );
    }

    public Task RevokeAsync(string refreshToken)
        => RevokeAsync(refreshToken, replacedByToken: null);

    public async Task RevokeAsync(string refreshToken, string? replacedByToken)
    {
        var e = await _db.Set<RefreshToken>().FirstOrDefaultAsync(t => t.Token == refreshToken);
        if (e is null) return; // idempotente

        if (e.RevokedAt is null)
        {
            e.RevokedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(replacedByToken))
                e.ReplacedByToken = replacedByToken;

            await _db.SaveChangesAsync();
        }
    }

    private static string GenerateSecureToken()
    {
        Span<byte> buf = stackalloc byte[64];  // 512 bits
        RandomNumberGenerator.Fill(buf);
        var b64 = Convert.ToBase64String(buf);
        return b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}