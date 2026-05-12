using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ScanRepository : IScanRepository
{
    private readonly VulnWatchDbContext _db;

    public ScanRepository(VulnWatchDbContext db)
    {
        _db = db;
    }

    public async Task<Scan?> GetByIdempotencyKeyAsync(Guid key, CancellationToken ct = default)
        => await _db.Scans.FirstOrDefaultAsync(s => s.IdempotencyKey == key, ct);

    public async Task<Scan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Scans.FindAsync(new object[] { id }, cancellationToken: ct);

    public async Task<Scan?> GetByIdWithFindingsAsync(Guid scanId, CancellationToken ct = default)
        => await _db.Scans
            .Include(s => s.Findings)
            .FirstOrDefaultAsync(s => s.Id == scanId, ct);

    public async Task<bool> HasActiveForDomainAsync(Guid domainId, CancellationToken ct = default)
        => await _db.Scans.AnyAsync(
            s => s.DomainId == domainId && s.Status == ScanStatus.Queued || s.Status == ScanStatus.Running,
            ct);

    public async Task AddAsync(Scan scan, CancellationToken ct = default)
        => await _db.Scans.AddAsync(scan, ct);

    public async Task UpdateAsync(Scan scan, CancellationToken ct = default)
    {
        _db.Scans.Update(scan);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<Scan>> GetByDomainIdAsync(Guid domainId, int page, int pageSize, CancellationToken ct = default)
        => await _db.Scans
            .Where(s => s.DomainId == domainId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(s => s.Findings)
            .ToListAsync(ct);

    public async Task<int> CountByDomainIdAsync(Guid domainId, CancellationToken ct = default)
        => await _db.Scans.CountAsync(s => s.DomainId == domainId, ct);
}
