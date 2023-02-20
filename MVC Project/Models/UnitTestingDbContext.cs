using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MVC_Project.Models;

public partial class UnitTestingDbContext : DbContext
{
    public UnitTestingDbContext()
    {
    }

    public UnitTestingDbContext(DbContextOptions<UnitTestingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}