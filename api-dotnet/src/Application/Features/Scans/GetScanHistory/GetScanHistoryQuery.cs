using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Application.Interfaces;

namespace Application.Features.Scans.GetScanHistory;

public record ScanSummary(Guid ScanId, string Status, int? SecurityScore, DateTime? StartedAt, DateTime? CompletedAt, int FindingCount);

public record ScanHistoryResponse(List<ScanSummary> Items, int TotalCount, int Page, int PageSize);

public record GetScanHistoryQuery(Guid DomainId, Guid RequestingUserId, int Page, int PageSize)
    : IRequest<Result<ScanHistoryResponse>>;

public class GetScanHistoryHandler : IRequestHandler<GetScanHistoryQuery, Result<ScanHistoryResponse>>
{
    private readonly IScanRepository _scanRepository;
    private readonly IDomainRepository _domainRepository;

    public GetScanHistoryHandler(IScanRepository scanRepository, IDomainRepository domainRepository)
    {
        _scanRepository = scanRepository;
        _domainRepository = domainRepository;
    }

    public async Task<Result<ScanHistoryResponse>> Handle(GetScanHistoryQuery query, CancellationToken cancellationToken)
    {
        // Verify domain exists and is owned by the requesting user
        var domain = await _domainRepository.GetByIdAsync(query.DomainId, cancellationToken);
        if (domain is null)
            return Result<ScanHistoryResponse>.Failure(Error.NotFound("Domain not found."));

        if (domain.UserId != query.RequestingUserId)
            return Result<ScanHistoryResponse>.Failure(Error.Forbidden("You do not have access to this domain."));

        // Get paginated scans
        var scans = await _scanRepository.GetByDomainIdAsync(query.DomainId, query.Page, query.PageSize, cancellationToken);
        var totalCount = await _scanRepository.CountByDomainIdAsync(query.DomainId, cancellationToken);

        // Map to response DTOs
        var items = scans.Select(s => new ScanSummary(
            s.Id,
            s.Status.ToString(),
            s.SecurityScore,
            s.StartedAt,
            s.CompletedAt,
            s.Findings.Count
        )).ToList();

        return Result<ScanHistoryResponse>.Success(new ScanHistoryResponse(items, totalCount, query.Page, query.PageSize));
    }
}
