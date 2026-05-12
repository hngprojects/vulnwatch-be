using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Scans.GetScanDetail;

public class GetScanDetailQueryHandler : IRequestHandler<GetScanDetailQuery, Result<ScanDetailResponse>>
{
    private readonly IScanRepository _scanRepository;
    private readonly IDomainRepository _domainRepository;

    public GetScanDetailQueryHandler(IScanRepository scanRepository, IDomainRepository domainRepository)
    {
        _scanRepository = scanRepository;
        _domainRepository = domainRepository;
    }

    public async Task<Result<ScanDetailResponse>> Handle(GetScanDetailQuery query, CancellationToken cancellationToken)
    {
        // 1. Load scan by ID
        var scan = await _scanRepository.GetByIdWithFindingsAsync(query.ScanId, cancellationToken);
        if (scan is null)
            return Result<ScanDetailResponse>.Failure(Error.NotFound("Scan not found."));

        // 2. Check access — scan must belong to requesting user
        if (scan.UserId != query.RequestingUserId)
            return Result<ScanDetailResponse>.Failure(Error.Forbidden("You do not have access to this scan."));

        // 3. Load domain to get domain name
        var domain = await _domainRepository.GetByIdAsync(scan.DomainId!.Value, cancellationToken);
        var domainName = domain?.DomainName ?? "Unknown";

        // 4. Map findings to DTOs, ordered by severity (Critical → High → Medium → Low)
        var severityOrder = new Dictionary<FindingSeverity, int>
        {
            { FindingSeverity.Critical, 0 },
            { FindingSeverity.High, 1 },
            { FindingSeverity.Medium, 2 },
            { FindingSeverity.Low, 3 }
        };

        var findingDtos = scan.Findings
            .OrderBy(f => severityOrder.TryGetValue(f.Severity, out var order) ? order : 4)
            .Select(f => new FindingDetailDto(
                f.Id,
                f.Surface.ToString(),
                f.Severity.ToString(),
                f.Title,
                f.CveId,
                f.AiExplanation,
                f.RemediationSteps,
                f.Status.ToString()
            ))
            .ToList();

        // 5. Build summary — count findings by severity
        var summary = new ScanDetailSummary(
            TotalFindings: scan.Findings.Count,
            Critical: scan.Findings.Count(f => f.Severity == FindingSeverity.Critical),
            High: scan.Findings.Count(f => f.Severity == FindingSeverity.High),
            Medium: scan.Findings.Count(f => f.Severity == FindingSeverity.Medium),
            Low: scan.Findings.Count(f => f.Severity == FindingSeverity.Low)
        );

        // 6. Build response
        var response = new ScanDetailResponse(
            ScanId: scan.Id,
            Status: scan.Status.ToString(),
            DomainName: domainName,
            SecurityScore: scan.SecurityScore,
            StartedAt: scan.StartedAt,
            CompletedAt: scan.CompletedAt,
            Summary: summary,
            Findings: findingDtos
        );

        return Result<ScanDetailResponse>.Success(response);
    }
}
