using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Remediation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FindingId { get; set; }
    public int? PrNumber { get; set; }
    public string? PrUrl { get; set; }
    public string Status { get; set; } = "open"; // open | merged | closed | failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Finding Finding { get; set; } = default!;
}
