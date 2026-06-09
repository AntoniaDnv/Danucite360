using System;
using System.Collections.Generic;
using System.Text;
using Danucite360.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Danucite360.Data.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Region> Regions { get; set; } = null!;

    public DbSet<BudgetCategory> BudgetCategories { get; set; } = null!;

    public DbSet<BudgetSource> BudgetSources { get; set; } = null!;

    public DbSet<BudgetRecord> BudgetRecords { get; set; } = null!;

    public DbSet<FundedProject> FundedProjects { get; set; } = null!;

    public DbSet<ProjectFunding> ProjectFundings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Region>()
            .HasIndex(r => r.Slug)
            .IsUnique();

        builder.Entity<BudgetCategory>()
            .HasIndex(c => c.Slug)
            .IsUnique();

        builder.Entity<BudgetRecord>()
            .Property(r => r.Amount)
            .HasPrecision(18, 2);

        builder.Entity<BudgetRecord>()
            .HasOne(r => r.Region)
            .WithMany(r => r.BudgetRecords)
            .HasForeignKey(r => r.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BudgetRecord>()
            .HasOne(r => r.BudgetCategory)
            .WithMany(c => c.BudgetRecords)
            .HasForeignKey(r => r.BudgetCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BudgetRecord>()
            .HasOne(r => r.BudgetSource)
            .WithMany(s => s.BudgetRecords)
            .HasForeignKey(r => r.BudgetSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FundedProject>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<FundedProject>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2);

        builder.Entity<FundedProject>()
            .HasOne(p => p.Region)
            .WithMany()
            .HasForeignKey(p => p.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FundedProject>()
            .HasOne(p => p.BudgetSource)
            .WithMany()
            .HasForeignKey(p => p.BudgetSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProjectFunding>()
            .Property(f => f.Amount)
            .HasPrecision(18, 2);

        builder.Entity<ProjectFunding>()
            .HasOne(f => f.FundedProject)
            .WithMany(p => p.FundingShares)
            .HasForeignKey(f => f.FundedProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}