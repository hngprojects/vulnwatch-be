namespace Hng.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Domain
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string DomainName { get; set; } = default!;
    public string? VerificationToken { get; set; }
    public string VerificationStatus { get; set; } = "pending"; // pending | verified
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = default!;
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
}