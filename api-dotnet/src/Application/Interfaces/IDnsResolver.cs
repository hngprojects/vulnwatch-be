namespace Application.Interfaces;

public interface IDnsResolver
{
    Task<IReadOnlyList<string>> GetTxtRecords(string host, CancellationToken ct);
}