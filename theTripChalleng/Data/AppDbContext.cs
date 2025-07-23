using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using theTripChalleng.Models;

namespace theTripChalleng.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<PointRequest> PointRequests { get; set; }

    public virtual DbSet<PointsHistory> PointsHistories { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    public virtual DbSet<User> Users { get; set; }

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Category_pkey");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryName).HasColumnType("character varying");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Criteria_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("Criteria_CategoryID_fkey");
        });

        modelBuilder.Entity<PointRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PointRequests_pkey");

            entity.Property(e => e.AdminComment).HasColumnType("character varying");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Proof).HasColumnType("character varying");

            entity.HasOne(d => d.Criteria).WithMany(p => p.PointRequests)
                .HasForeignKey(d => d.CriteriaId)
                .HasConstraintName("PointRequests_CriteriaId_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.PointRequests)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("PointRequests_StatusId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PointRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PointRequests_UserId_fkey");
        });

        modelBuilder.Entity<PointsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PointsHistory_pkey");

            entity.ToTable("PointsHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApprovedBy).HasColumnType("character varying");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Criteria).WithMany(p => p.PointsHistories)
                .HasForeignKey(d => d.CriteriaId)
                .HasConstraintName("PointsHistory_CriteriaId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PointsHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("PointsHistory_UserId_fkey");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RequestStatus_pkey");

            entity.ToTable("RequestStatus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.StatusName).HasColumnType("character varying");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rewards_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnType("character varying");
            entity.Property(e => e.Name).HasColumnType("character varying");

            entity.HasOne(d => d.Category).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("Rewards_CategoryId_fkey");
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rules_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.RuleName).HasColumnType("character varying");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.Password).HasColumnType("character varying");
            entity.Property(e => e.Phone)
                .HasColumnType("character varying")
                .HasColumnName("phone");
            entity.Property(e => e.TotalPoints).HasColumnName("totalPoints");

            entity.HasOne(d => d.Rule).WithMany(p => p.Users)
                .HasForeignKey(d => d.RuleId)
                .HasConstraintName("User_RuleId_fkey");
        });
        modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
