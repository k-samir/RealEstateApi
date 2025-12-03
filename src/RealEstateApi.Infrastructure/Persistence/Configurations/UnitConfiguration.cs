using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;

namespace RealEstateApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Unit entity
/// </summary>
public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("unit");

        // Configure EF Core to use field-based access for private setters
        builder.UsePropertyAccessMode(PropertyAccessMode.Field);

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
            .HasConversion<string>() // Store enum as string in database
            .HasDefaultValue(UnitStatus.Available);

        // Media & Marketing - Store as JSON
        builder.Property(u => u.Images)
            .HasColumnName("images")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(u => u.FloorPlans)
            .HasColumnName("floor_plans")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(u => u.Amenities)
            .HasColumnName("amenities")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(u => u.Description)
            .HasColumnName("description")
            .HasColumnType("text");

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
