using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hng.Domain.Entities;

public class Waitlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = default!;
    public string? CompanyName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
