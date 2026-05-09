using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class NotificationPreferences
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }             // one-to-one with User
    public bool EmailAlerts { get; set; } = true;
    public bool SlackAlerts { get; set; } = false;
    public bool PushNotifications { get; set; } = false;

    public User User { get; set; } = default!;
}
