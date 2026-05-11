using Domain.Entities;

namespace Application.Interfaces;

public interface IDomainRepository
{
    Task<ScannedDomain?> GetByNameAsync(string domain, CancellationToken ct = default);
    Task<ScannedDomain?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
