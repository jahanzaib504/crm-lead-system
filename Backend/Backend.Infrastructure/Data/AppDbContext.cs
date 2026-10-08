using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Models;

namespace Backend.Infrastructure.Data;

public partial class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<FollowUp> FollowUps { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<LeadEvaluation> LeadEvaluations { get; set; }

    public virtual DbSet<LeadStage> LeadStages { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasIndex(e => new { e.LeadId, e.ActivityDate }, "IX_Activities_LeadDate").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.ActivityType).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Activities_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Activities_IsActive");
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.Lead).WithMany(p => p.Activities)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activities_Leads");

            entity.HasOne(d => d.LoggedByUser).WithMany(p => p.Activities)
                .HasForeignKey(d => d.LoggedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activities_Users");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(e => e.Action).HasMaxLength(20);
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(sysutcdatetime())", "DF_AuditLogs_Timestamp");

            entity.HasOne(d => d.PerformedByUser).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.PerformedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLogs_Users");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasIndex(e => e.Name, "UX_Companies_Name")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.AnnualRevenue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Companies_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.Industry).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Companies_IsActive");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasIndex(e => e.Email, "UX_Contacts_Email")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Contacts_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Contacts_IsActive");
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Contacts_Companies");
        });

        modelBuilder.Entity<FollowUp>(entity =>
        {
            entity.HasIndex(e => new { e.AssignedToUserId, e.Status, e.DueDate }, "IX_FollowUps_AssignedStatus").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_FollowUps_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_FollowUps_IsActive");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Medium");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.FollowUps)
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FollowUps_Users");

            entity.HasOne(d => d.Lead).WithMany(p => p.FollowUps)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FollowUps_Leads");
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasIndex(e => new { e.AssignedToUserId, e.LeadStageId }, "IX_Leads_AssignedStage").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Title, "IX_Leads_Title").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Leads_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.CurrentScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EstimatedValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Leads_IsActive");
            entity.Property(e => e.Source).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.Leads)
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Leads_Users");

            entity.HasOne(d => d.Company).WithMany(p => p.Leads)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Leads_Companies");

            entity.HasOne(d => d.Contact).WithMany(p => p.Leads)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Leads_Contacts");

            entity.HasOne(d => d.LeadStage).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadStageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Leads_LeadStages");
        });

        modelBuilder.Entity<LeadEvaluation>(entity =>
        {
            entity.Property(e => e.CalculatedScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ConfidenceRating).HasMaxLength(20);
            entity.Property(e => e.EvaluatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_LeadEvaluations_EvaluatedAt");
            entity.Property(e => e.OverriddenScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.OverrideReason).HasMaxLength(500);

            entity.HasOne(d => d.EvaluatedByUser).WithMany(p => p.LeadEvaluations)
                .HasForeignKey(d => d.EvaluatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadEvaluations_Users");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadEvaluations)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadEvaluations_Leads");
        });

        modelBuilder.Entity<LeadStage>(entity =>
        {
            entity.HasIndex(e => e.Name, "UX_LeadStages_Name")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.ConversionWeight).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_LeadStages_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_LeadStages_IsActive");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Notes_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Notes_IsActive");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.Lead).WithMany(p => p.Notes)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notes_Leads");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UX_Users_Email")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("SystemSeed");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.ManagerId).HasColumnName("managerId");
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_MANAGER_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
