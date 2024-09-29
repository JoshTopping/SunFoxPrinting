using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Orleans.ShoppingCart.Silo;

public partial class SunfoxPrintingContext : DbContext
{
    public SunfoxPrintingContext()
    {
    }

    public SunfoxPrintingContext(DbContextOptions<SunfoxPrintingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=SunfoxPrinting;Username=postgres;Password=3Tertles");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("Clients_pkey");

            entity.Property(e => e.ClientId).UseIdentityAlwaysColumn();
            entity.Property(e => e.ClientEmail).HasMaxLength(255);
            entity.Property(e => e.ClientName).HasMaxLength(255);
            entity.Property(e => e.ClientPhone).HasMaxLength(255);
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("Photos_pkey");

            entity.Property(e => e.ClientId).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(15);
            entity.Property(e => e.PhotoId)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn();
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Client).WithOne(p => p.Photo)
                .HasForeignKey<Photo>(d => d.ClientId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_client");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
