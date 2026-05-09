using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hng.Domain.Entities;

public class Scan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid IdempotencyKey { get; set; }
    public string TargetType { get; set; } = "domain"; // domain | repository
    public Guid? DomainId { get; set; }
    public string Status { get; set; } = "queued";   // queued|running|completed|failed
    public int? SecurityScore { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ScannedDomain? Domain { get; set; }
    public User User { get; set; } = default!;
    public ICollection<Finding> Findings { get; set; } = new List<Finding>();
}