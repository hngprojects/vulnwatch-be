using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VulnWatchDbContext : DbContext
{
    public VulnWatchDbContext(DbContextOptions<VulnWatchDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Waitlist> Waitlists => Set<Waitlist>();
    public DbSet<ScannedDomain> Domains => Set<ScannedDomain>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<Finding> Findings => Set<Finding>();
    public DbSet<Remediation> Remediations => Set<Remediation>();
    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<MonitoredRepository> MonitoredRepositories => Set<MonitoredRepository>();
    public DbSet<NotificationPreferences> NotificationPreferences => Set<NotificationPreferences>();
    public DbSet<WebHookOutBox> WebHookOutBox => Set<WebHookOutBox>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.GoogleId).IsUnique();
            e.Property(u => u.Email).IsRequired();
        });

        builder.Entity<ScannedDomain>(e =>
        {
            e.Property(d => d.VerificationStatus).HasConversion<string>();
            e.HasOne(d => d.User)
             .WithMany()
             .HasForeignKey(d => d.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Scan>(e =>
        {
            e.HasIndex(s => s.IdempotencyKey).IsUnique();
            e.Property(s => s.TargetType).HasConversion<string>();
            e.Property(s => s.Status).HasConversion<string>();
            e.HasOne(s => s.Domain)
             .WithMany(d => d.Scans)
             .HasForeignKey(s => s.DomainId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.User)
             .WithMany()
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Finding>(e =>
        {
            e.Property(f => f.Surface).HasConversion<string>();
            e.Property(f => f.Severity).HasConversion<string>();
            e.Property(f => f.Status).HasConversion<string>();
            e.HasOne(f => f.Scan)
             .WithMany(s => s.Findings)
             .HasForeignKey(f => f.ScanId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Remediation>(e =>
        {
            e.Property(r => r.Status).HasConversion<string>();
            e.HasOne(r => r.Finding)
             .WithMany()
             .HasForeignKey(r => r.FindingId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Integration>(e =>
        {
            e.Property(i => i.Status).HasConversion<string>();
            e.HasOne(i => i.User)
             .WithMany()
             .HasForeignKey(i => i.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<MonitoredRepository>(e =>
        {
            e.HasIndex(r => r.RepoId).IsUnique();
            e.HasOne(r => r.User)
             .WithMany()
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<NotificationPreferences>(e =>
        {
            e.HasOne(n => n.User)
             .WithMany()
             .HasForeignKey(n => n.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<WebHookOutBox>(e =>
        {
            e.Property(w => w.Status).HasConversion<string>();
        });
    }
}
