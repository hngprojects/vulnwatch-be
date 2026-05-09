using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Integration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Provider { get; set; } = default!;       // github | slack
    public string InstallationId { get; set; } = default!; // integration-specific token ID
    public IntegrationStatus Status { get; set; } = IntegrationStatus.INACTIVE;         // active | disconnected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = default!;
}