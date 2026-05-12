using Application.Interfaces;
using DnsClient;

namespace Infrastructure.Services;

public sealed class DnsResolver(ILookupClient dns) : IDnsResolver
{
    public async Task<IReadOnlyList<string>> GetTxtRecords(string host, CancellationToken ct)
    {
        var result = await dns.QueryAsync(host, QueryType.TXT, cancellationToken: ct);

        return result.Answers
            .TxtRecords()
            .SelectMany(r => r.Text)
            .ToList();
    }
}