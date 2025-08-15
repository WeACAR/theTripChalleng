using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using theTripChalleng.Models;

namespace theTripChalleng.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AssignedCritera> AssignedCriteras { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Criterion> Criteria { get; set; } = null!;
        public virtual DbSet<Email> Emails { get; set; } = null!;
        public virtual DbSet<Nickname> Nicknames { get; set; } = null!;
        public virtual DbSet<PointRequest> PointRequests { get; set; } = null!;
        public virtual DbSet<PointsHistory> PointsHistories { get; set; } = null!;
        public virtual DbSet<RequestStatus> RequestStatuses { get; set; } = null!;
        public virtual DbSet<Reward> Rewards { get; set; } = null!;
        public virtual DbSet<Rule> Rules { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=db.nwrvbybfuuheflahamro.supabase.co;Database=postgres;Username=postgres;Password=4wTGrPurRenesQ0W;SSL Mode=Require;Trust Server Certificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
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

            modelBuilder.Entity<AssignedCritera>(entity =>
            {
                entity.ToTable("AssignedCritera");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName).HasColumnType("character varying");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Criterion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CriteriaName).HasColumnType("character varying");

                entity.Property(e => e.IsAssignable).HasColumnName("isAssignable");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Criteria)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Criteria_CategoryID_fkey");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.ReceiverNavigation)
                    .WithMany(p => p.EmailReceiverNavigations)
                    .HasForeignKey(d => d.Receiver)
                    .HasConstraintName("Emails_Receiver_fkey");

                entity.HasOne(d => d.SenderNavigation)
                    .WithMany(p => p.EmailSenderNavigations)
                    .HasForeignKey(d => d.Sender)
                    .HasConstraintName("Emails_Sender_fkey");
            });

            modelBuilder.Entity<Nickname>(entity =>
            {
                entity.ToTable("Nickname");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Nicknames)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("Nickname_UserId_fkey");
            });

            modelBuilder.Entity<PointRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PointRequests_pkey");

                entity.Property(e => e.AdminComment).HasColumnType("character varying");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Proof).HasColumnType("character varying");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.PointRequests)
                    .HasForeignKey(d => d.CriteriaId)
                    .HasConstraintName("PointRequests_CriteriaId_fkey");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PointRequests)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("PointRequests_StatusId_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PointRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PointRequests_UserId_fkey");
            });

            modelBuilder.Entity<PointsHistory>(entity =>
            {
                entity.ToTable("PointsHistory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprovedBy).HasColumnType("character varying");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.PointsHistories)
                    .HasForeignKey(d => d.CriteriaId)
                    .HasConstraintName("PointsHistory_CriteriaId_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PointsHistories)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("PointsHistory_UserId_fkey");
            });

            modelBuilder.Entity<RequestStatus>(entity =>
            {
                entity.ToTable("RequestStatus");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.StatusName).HasColumnType("character varying");
            });

            modelBuilder.Entity<Reward>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.Name).HasColumnType("character varying");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Rewards)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Rewards_CategoryId_fkey");
            });

            modelBuilder.Entity<Rule>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.RuleName).HasColumnType("character varying");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Name).HasColumnType("character varying");

                entity.Property(e => e.Password).HasColumnType("character varying");

                entity.Property(e => e.Phone)
                    .HasColumnType("character varying")
                    .HasColumnName("phone");

                entity.Property(e => e.TotalPoints).HasColumnName("totalPoints");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RuleId)
                    .HasConstraintName("User_RuleId_fkey");
            });

            modelBuilder.HasSequence<int>("seq_schema_version", "graphql").IsCyclic();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
