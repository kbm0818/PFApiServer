using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PFApiServer.Models.Global;

public partial class GlobalContext : DbContext
{
    public GlobalContext(DbContextOptions<GlobalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Nickname> Nicknames { get; set; }

    public virtual DbSet<Sharding> Shardings { get; set; }

    public virtual DbSet<System> Systems { get; set; }

    public virtual DbSet<SystemMail> SystemMails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nickname>(entity =>
        {
            entity.HasKey(e => e.SerialNo).HasName("PRIMARY");

            entity.ToTable("Nickname", "PFGlobal");

            entity.HasIndex(e => e.Name, "name");

            entity.HasIndex(e => new { e.Uid, e.AccountSerialNo }, "uid, acc_serial");

            entity.Property(e => e.Name).HasMaxLength(10);
        });

        modelBuilder.Entity<Sharding>(entity =>
        {
            entity.HasKey(e => e.ShardingIdx).HasName("PRIMARY");

            entity.ToTable("Sharding", "PFGlobal");

            entity.Property(e => e.ShardingIdx).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<System>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("System", "PFGlobal");

            entity.Property(e => e.Version)
                .HasMaxLength(32)
                .HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<SystemMail>(entity =>
        {
            entity.HasKey(e => e.SerialNo).HasName("PRIMARY");

            entity.ToTable("SystemMail", "PFGlobal");

            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.ItemList).HasColumnType("json");
            entity.Property(e => e.RemainDays).HasDefaultValueSql("'7'");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.SubTitle).HasMaxLength(256);
            entity.Property(e => e.Title).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
