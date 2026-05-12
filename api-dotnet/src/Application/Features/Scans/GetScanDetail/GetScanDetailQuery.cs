using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Scans.GetScanDetail;

public record FindingDetailDto(
    Guid Id,
    string Surface,
    string Severity,
    string Title,
    string? CveId,
    string? AiExplanation,
    string? RemediationSteps,
    string Status
);

public record ScanDetailSummary(
    int TotalFindings,
    int Critical,
    int High,
    int Medium,
    int Low
);

public record ScanDetailResponse(
    Guid ScanId,
    string Status,
    string DomainName,
    int? SecurityScore,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    ScanDetailSummary Summary,
    List<FindingDetailDto> Findings
);

public record GetScanDetailQuery(Guid ScanId, Guid RequestingUserId)
    : IRequest<Result<ScanDetailResponse>>;
