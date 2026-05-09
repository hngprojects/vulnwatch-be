using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hng.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hng.Infrastructure.Persistence;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Users
        builder.Entity<User>(e => {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.GoogleId).IsUnique();
            e.Property(u => u.Email).IsRequired();
        });

        // Domains — cascade delete
        builder.Entity<ScannedDomain>(e => {
            e.HasKey(d => d.Id);
            e.HasOne(d => d.User)
             .WithMany()
             .HasForeignKey(d => d.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Scans
        builder.Entity<Scan>(e => {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.IdempotencyKey).IsUnique();
            e.HasOne(s => s.Domain)
             .WithMany(d => d.Scans)
             .HasForeignKey(s => s.DomainId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Findings — cascade from scan
        builder.Entity<Finding>(e => {
            e.HasKey(f => f.Id);
            e.HasOne(f => f.Scan)
             .WithMany(s => s.Findings)
             .HasForeignKey(f => f.ScanId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
