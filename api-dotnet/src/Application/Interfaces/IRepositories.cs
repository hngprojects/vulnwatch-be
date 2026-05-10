



using Domain.Entities;

namespace Application.Interfaces;


public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetById(Guid id, CancellationToken ct = default);
    Task Add(RefreshToken token, CancellationToken ct = default);
    Task Update(RefreshToken token, CancellationToken ct = default);
    Task Delete(RefreshToken token, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
    Task<RefreshToken?> GetByToken(string rawToken, CancellationToken ct = default);
    Task<IEnumerable<RefreshToken>> GetActiveByUserId(Guid userId, CancellationToken ct = default);

}