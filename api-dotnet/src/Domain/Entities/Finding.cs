using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hng.Domain.Entities;

public class Finding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ScanId { get; set; }
    public string Surface { get; set; } = default!; // HTTP_HEADERS|SSL|DNS|DEPENDENCY
    public string Severity { get; set; } = default!; // Critical|High|Medium|Low
    public string Title { get; set; } = default!;
    public string? CveId { get; set; }
    public string? AiExplanation { get; set; }
    public string? TechnicalPayload { get; set; }
    public string? RemediationSteps { get; set; }
    public string Status { get; set; } = "open"; // open|remediated|ignored

    public Scan Scan { get; set; } = default!;
}
