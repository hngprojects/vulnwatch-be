using MediatR;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Application.Interfaces;

namespace Application.Features.Scans;


// requestDto: What the API receives from the client (Intern-friendly: Shape of the incoming data)
public record CreateScanRequest(string Domain, string[] ScanTypes);

// Command: What the user wants to do (Intern-friendly: Data coming in) - this is what the handler will process
public record CreateScanCommand(Guid IdempotencyKey, string Domain, string[] ScanTypes)
    : IRequest<Result<ScanResponse>>;

// DTO: Data Transfer Object (Intern-friendly: Shape of the response)
public record ScanResponse(Guid ScanId, string Status);

// Handler: The "Brain" of the operation (Intern-friendly: Where the logic happens)
public class CreateScanHandler : IRequestHandler<CreateScanCommand, Result<ScanResponse>>
{
    private readonly IRedisProducer _redisProducer;
    private readonly IScanRepository _scanRepository;
    private readonly IDomainRepository _domainRepository;
    private readonly ICurrentUser _currentUser;

    public CreateScanHandler(IRedisProducer redisProducer, IScanRepository scanRepository, IDomainRepository domainRepository, ICurrentUser currentUser)
    {
        _redisProducer = redisProducer;
        _scanRepository = scanRepository;
        _domainRepository = domainRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<ScanResponse>> Handle(CreateScanCommand request, CancellationToken cancellationToken)
    {
        // 1. Guard against duplicate submissions — same idempotency key means the client already sent this request
        var existing = await _scanRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
        if (existing is not null)
            return Result<ScanResponse>.Success(new ScanResponse(existing.Id, existing.Status.ToString()));

        var userId = _currentUser.UserId;

        // 2. Load domain by name and verify it exists
        var domain = await _domainRepository.GetByNameAsync(request.Domain, cancellationToken);
        if (domain is null)
            return Result<ScanResponse>.Failure(Error.NotFound($"Domain '{request.Domain}' not found."));

        // 3. Check domain ownership
        if (domain.UserId != userId)
            return Result<ScanResponse>.Failure(Error.Forbidden("You do not have access to this domain."));

        // 4. Check domain is verified
        if (domain.VerificationStatus != VerificationStatus.Verified)
            return Result<ScanResponse>.Failure(Error.Validation("Domain must be verified before scanning."));

        // 5. Check no active scan is running for this domain
        var hasActive = await _scanRepository.HasActiveForDomainAsync(domain.Id, cancellationToken);
        if (hasActive)
            return Result<ScanResponse>.Failure(Error.Conflict("A scan is already running for this domain."));

        // 6. Build the scan entity
        var scan = Scan.Create(
            userId: userId,
            idempotencyKey: request.IdempotencyKey,
            targetType: ScanTargetType.Domain,
            domainId: domain.Id
        );

        // 7. Persist scan to DB before publishing so the record exists if the worker picks it up immediately
        await _scanRepository.AddAsync(scan, cancellationToken);

        // 8. Push the job to Redis Queue for the scanner worker to pick up
        await _redisProducer.PublishAsync("scan-jobs", new
        {
            scan_id = scan.Id,
            domain = request.Domain,
            scan_types = request.ScanTypes,
            requested_by = userId,
            enqueuedAt = DateTime.UtcNow,
        });

        // 9. Return the full result — controller maps IsSuccess/Error to the correct HTTP status code
        return Result<ScanResponse>.Success(new ScanResponse(scan.Id, scan.Status.ToString()));
    }
}
