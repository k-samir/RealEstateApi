using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Unit entity
/// </summary>
public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("unit");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        // Unit Details
        builder.Property(u => u.UnitNumber)
            .HasColumnName("unit_number")
            .IsRequired();

        builder.Property(u => u.Floor)
            .HasColumnName("floor");

        builder.Property(u => u.Type)
            .HasColumnName("type")
            .IsRequired();

        // Specifications
        builder.Property(u => u.Bedrooms)
            .HasColumnName("bedrooms")
            .IsRequired();

        builder.Property(u => u.Bathrooms)
            .HasColumnName("bathrooms")
            .IsRequired();

        builder.Property(u => u.Area)
            .HasColumnName("area")
            .HasPrecision(10, 2)
            .IsRequired();

        // Pricing & Availability
        builder.Property(u => u.Price)
            .HasColumnName("price")
            .HasPrecision(15, 2)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue("Available");

        // Metadata
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(u => u.PropertyId);
        builder.HasIndex(u => u.Status);
    }
}
