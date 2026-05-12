using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ScannedDomainRepository(VulnWatchDbContext db)
    : BaseRepository<ScannedDomain>(db), IScannedDomainRepository
{
    public Task<ScannedDomain?> FindActive(string domain, CancellationToken ct) =>
        Db.Domains
            .FirstOrDefaultAsync(d =>
                d.DomainName == domain &&
                d.VerificationStatus != VerificationStatus.Revoked, ct);

    public Task<int> CountPending(Guid userId, CancellationToken ct) =>
        Db.Domains
            .CountAsync(d =>
                d.UserId == userId &&
                d.VerificationStatus == VerificationStatus.Pending, ct);

    public Task<ScannedDomain?> FindPendingById(Guid id, Guid userId, CancellationToken ct) =>
        Db.Domains
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.UserId == userId &&
                d.VerificationStatus == VerificationStatus.Pending, ct);

}