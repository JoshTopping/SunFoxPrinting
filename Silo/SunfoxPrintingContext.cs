using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Orleans.ShoppingCart.Silo;

public partial class SunfoxPrintingContext : DbContext
{

    // requires using Microsoft.Extensions.Configuration;
    private readonly IConfiguration Configuration;

    public SunfoxPrintingContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public SunfoxPrintingContext(DbContextOptions<SunfoxPrintingContext> options, IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //IConfigurationRoot configuration = new ConfigurationBuilder()
        //   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //   .AddJsonFile("appsettings.json")
        //   .Build();
        ////optionsBuilder.UseNpgsql("Name=SunfoxPrintingContext:ConnectionString");
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
    }

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
