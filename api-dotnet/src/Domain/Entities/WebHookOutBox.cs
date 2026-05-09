using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;


public class WebHookOutBox
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MessageBody { get; set; } = default!;
    public int NumRetries { get; set; } = 0;
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
    public DateTime? DeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
