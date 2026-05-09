using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly VulnWatchDbContext _db;

    public RefreshTokenRepository(VulnWatchDbContext db)
    {
        _db = db;
    }

    public async Task<RefreshToken?> GetById(Guid id, CancellationToken ct = default)
        => await _db.RefreshTokens.FindAsync([id], ct);

    public async Task<RefreshToken?> GetByToken(string tokenHash, CancellationToken ct = default)
        => await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task<IEnumerable<RefreshToken>> GetActiveByUserId(Guid userId, CancellationToken ct = default)
        => await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

    public async Task Add(RefreshToken token, CancellationToken ct = default)
        => await _db.RefreshTokens.AddAsync(token, ct);

    public Task Update(RefreshToken token, CancellationToken ct = default)
    {
        _db.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public Task Delete(RefreshToken token, CancellationToken ct = default)
    {
        _db.RefreshTokens.Remove(token);
        return Task.CompletedTask;
    }

    public async Task SaveChanges(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}