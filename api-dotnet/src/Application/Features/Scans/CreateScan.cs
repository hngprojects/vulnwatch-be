namespace Application.Features.Scans;

using MediatR;
using Application.Interfaces;

// Command: What the user wants to do (Intern-friendly: Data coming in)
public record CreateScanCommand(string Domain, string[] ScanTypes) : IRequest<Guid>;

// DTO: Data Transfer Object (Intern-friendly: Shape of the response)
public record ScanResponse(Guid ScanId, string Status);

// Handler: The "Brain" of the operation (Intern-friendly: Where the logic happens)
public class CreateScanHandler : IRequestHandler<CreateScanCommand, Guid>
{
    private readonly IRedisProducer _redisProducer;

    public CreateScanHandler(IRedisProducer redisProducer)
    {
        _redisProducer = redisProducer;
    }

    public async Task<Guid> Handle(CreateScanCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate a unique ID for the scan
        var scanId = Guid.NewGuid();
        
        // 2. TODO: Save scan record to PostgreSQL Database
        
        // 3. Push the job to Redis Queue for the Java worker to pick up
        await _redisProducer.PublishAsync("scan-jobs", new {
            scan_id = scanId,
            domain = request.Domain,
            scan_type = request.ScanTypes,
            requested_by = "anonymous_user" // Replace with real User ID after Auth is implemented
        });

        return scanId;
    }
}
