using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using AxciomCRM.Models;

namespace AxciomCRM.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Opportunity> Opportunities { get; set; }
    public DbSet<FollowUp> FollowUps { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
        builder.Entity<Customer>().HasIndex(c => c.Phone).IsUnique();
        builder.Entity<Lead>().Property(l => l.ExpectedValue).HasPrecision(18, 2);
        builder.Entity<Opportunity>().Property(o => o.Amount).HasPrecision(18, 2);
    }
}
