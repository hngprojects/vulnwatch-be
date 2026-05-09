using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class MonitoredRepository
{
    public long RepoId { get; set; }              // GitHub's unique repo ID (bigint PK)
    public Guid UserId { get; set; }
    public string FullName { get; set; } = default!; // e.g. "owner/repo"
    public bool IsMonitoringActive { get; set; } = true;
    public string DefaultBranch { get; set; } = "main";
    public long GitHubInstallationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = default!;
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
    public ICollection<VulnerabilityIntel> VulnerabilityIntels { get; set; } = new List<VulnerabilityIntel>();
}
