using System.Text.Json.Serialization;

namespace Application.Features.Scans.ProcessScanResult;

public record ScanResultMessage(
    [property: JsonPropertyName("scanId")] Guid ScanId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("securityScore")] int SecurityScore,
    [property: JsonPropertyName("findingCount")] int FindingCount
);
