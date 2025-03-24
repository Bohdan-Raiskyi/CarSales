using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CarSalesDomain.Model;

namespace CarSalesInfrastructure;

public partial class CarSalesContext : DbContext
{
    public CarSalesContext()
    {
    }

    public CarSalesContext(DbContextOptions<CarSalesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ad> Ads { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<CarType> CarTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<PriceRange> PriceRanges { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<SavedAd> SavedAds { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Bogdans_PC\\SQLEXPRESS; Database=CarSales; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ad__3213E83F0C342F72");

            entity.ToTable("Ad");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("creation_date");
            entity.Property(e => e.Description)
                .HasMaxLength(4000)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PriceRangeId).HasColumnName("price_range_id");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.SoldDate)
                .HasColumnType("datetime")
                .HasColumnName("sold_date");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Ads)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ad_CarBrand");

            entity.HasOne(d => d.PriceRange).WithMany(p => p.Ads)
                .HasForeignKey(d => d.PriceRangeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ad_PriceRange");

            entity.HasOne(d => d.Region).WithMany(p => p.Ads)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ad_Region");

            entity.HasOne(d => d.Type).WithMany(p => p.Ads)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ad_CarType");

            entity.HasOne(d => d.User).WithMany(p => p.Ads)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ad_User");
        });

        modelBuilder.Entity<CarBrand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Car_bran__3213E83F8FC48FC6");

            entity.ToTable("Car_brand");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandName)
                .HasMaxLength(100)
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<CarType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Car_type__3213E83F72D82840");

            entity.ToTable("Car_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarType1)
                .HasMaxLength(100)
                .HasColumnName("car_type");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3213E83F810C2163");

            entity.ToTable("Image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdId).HasColumnName("ad_id");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");

            entity.HasOne(d => d.Ad).WithMany(p => p.Images)
                .HasForeignKey(d => d.AdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_Ad");
        });

        modelBuilder.Entity<PriceRange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Price_ra__3213E83F3576BCE6");

            entity.ToTable("Price_range");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RangeLabel)
                .HasMaxLength(100)
                .HasColumnName("range_label");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Region__3213E83F0EA56AAD");

            entity.ToTable("Region");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RegName)
                .HasMaxLength(100)
                .HasColumnName("reg_name");
        });

        modelBuilder.Entity<SavedAd>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Saved_ad__3213E83FA47B0978");

            entity.ToTable("Saved_ads");

            entity.HasIndex(e => new { e.UserId, e.AdId }, "UQ_SavedAds_UserAd").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdId).HasColumnName("ad_id");
            entity.Property(e => e.SavedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("saved_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Ad).WithMany(p => p.SavedAds)
                .HasForeignKey(d => d.AdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedAds_Ad");

            entity.HasOne(d => d.User).WithMany(p => p.SavedAds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedAds_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F854D580B");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}