using Domain.Entities;

namespace Application.Interfaces;

public interface IScanRepository
{
    Task<Scan?> GetByIdempotencyKeyAsync(Guid key, CancellationToken ct = default);
    Task<Scan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Scan?> GetByIdWithFindingsAsync(Guid scanId, CancellationToken ct = default);
    Task<bool> HasActiveForDomainAsync(Guid domainId, CancellationToken ct = default);
    Task AddAsync(Scan scan, CancellationToken ct = default);
    Task UpdateAsync(Scan scan, CancellationToken ct = default);
    Task<List<Scan>> GetByDomainIdAsync(Guid domainId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByDomainIdAsync(Guid domainId, CancellationToken ct = default);
}
