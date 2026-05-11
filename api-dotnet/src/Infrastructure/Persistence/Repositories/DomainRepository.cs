using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DomainRepository : IDomainRepository
{
    private readonly VulnWatchDbContext _db;

    public DomainRepository(VulnWatchDbContext db)
    {
        _db = db;
    }

    public async Task<ScannedDomain?> GetByNameAsync(string domain, CancellationToken ct = default)
        => await _db.Domains.FirstOrDefaultAsync(d => d.DomainName == domain, ct);

    public async Task<ScannedDomain?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Domains.FirstOrDefaultAsync(d => d.Id == id, ct);
}
