using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;
using System.Text.Json;

namespace RealEstateApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Property entity
/// Maps domain entity to database schema
/// </summary>
public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("property");

        // Configure EF Core to use field-based access for private setters
        builder.UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.AgentId)
            .HasColumnName("agent_id")
            .IsRequired();

        // Basic Information
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(p => p.Location)
            .HasColumnName("location")
            .IsRequired();

        builder.Property(p => p.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(10, 7);

        builder.Property(p => p.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(10, 7);

        builder.Property(p => p.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>() // Store enum as string in database
            .HasDefaultValue(PropertyStatus.Draft);

        builder.Property(p => p.CompletionDate)
            .HasColumnName("completion_date");

        // Descriptions
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(p => p.LongDescription)
            .HasColumnName("long_description");

        // JSON columns for complex types
        builder.Property(p => p.Features)
            .HasColumnName("features")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            )
            .HasDefaultValue(new List<string>());

        builder.Property(p => p.Amenities)
            .HasColumnName("amenities")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Amenity>>(v, (JsonSerializerOptions?)null) ?? new List<Amenity>()
            )
            .HasDefaultValue(new List<Amenity>());

        builder.Property(p => p.Specifications)
            .HasColumnName("specifications")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Specification>>(v, (JsonSerializerOptions?)null) ?? new List<Specification>()
            )
            .HasDefaultValue(new List<Specification>());

        // Media
        builder.Property(p => p.MainImage)
            .HasColumnName("main_image");

        builder.Property(p => p.Images)
            .HasColumnName("images")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            )
            .HasDefaultValue(new List<string>());

        // Summary Fields
        builder.Property(p => p.BedroomsRange)
            .HasColumnName("bedrooms_range");

        builder.Property(p => p.BathroomsRange)
            .HasColumnName("bathrooms_range");

        builder.Property(p => p.AreaRange)
            .HasColumnName("area_range");

        builder.Property(p => p.PriceRange)
            .HasColumnName("price_range");

        // Metadata
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships - use backing field for private setter
        builder.Metadata.FindNavigation(nameof(Property.Units))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Units)
            .WithOne(u => u.Property)
            .HasForeignKey(u => u.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(p => p.AgentId);
        builder.HasIndex(p => p.Location);
        builder.HasIndex(p => p.Status);
    }
}
