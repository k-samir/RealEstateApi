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

        builder.Property(p => p.Developer)
            .HasColumnName("developer");

        builder.Property(p => p.Category)
            .HasColumnName("category");

        builder.Property(p => p.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>() // Store enum as string in database
            .HasDefaultValue(PropertyStatus.Draft);

        builder.Property(p => p.IsPublished)
            .HasColumnName("is_published")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsFeatured)
            .HasColumnName("is_featured")
            .IsRequired()
            .HasDefaultValue(false);

        // Location
        builder.Property(p => p.Location)
            .HasColumnName("location")
            .IsRequired();

        builder.Property(p => p.StreetAddress)
            .HasColumnName("street_address");

        builder.Property(p => p.Area)
            .HasColumnName("area");

        builder.Property(p => p.City)
            .HasColumnName("city");

        builder.Property(p => p.State)
            .HasColumnName("state");

        builder.Property(p => p.Country)
            .HasColumnName("country");

        builder.Property(p => p.PostalCode)
            .HasColumnName("postal_code");

        builder.Property(p => p.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(10, 7);

        builder.Property(p => p.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(10, 7);

        // Pricing
        builder.Property(p => p.PriceRange)
            .HasColumnName("price_range");

        // Project Details
        builder.Property(p => p.CompletionDate)
            .HasColumnName("completion_date");

        builder.Property(p => p.TotalUnits)
            .HasColumnName("total_units");

        builder.Property(p => p.TotalFloors)
            .HasColumnName("total_floors");

        builder.Property(p => p.TotalLandArea)
            .HasColumnName("total_land_area");

        builder.Property(p => p.LandAreaUnit)
            .HasColumnName("land_area_unit");

        builder.Property(p => p.UnitTypesAvailable)
            .HasColumnName("unit_types_available")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
            );

        // Unit Specifications
        builder.Property(p => p.BedroomsRange)
            .HasColumnName("bedrooms_range");

        builder.Property(p => p.BathroomsRange)
            .HasColumnName("bathrooms_range");

        builder.Property(p => p.AreaRange)
            .HasColumnName("area_range");

        builder.Property(p => p.FurnishingStatus)
            .HasColumnName("furnishing_status");

        // Descriptions
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(p => p.LongDescription)
            .HasColumnName("long_description");

        builder.Property(p => p.KeyHighlights)
            .HasColumnName("key_highlights")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
            );

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

        builder.Property(p => p.NearbyPlaces)
            .HasColumnName("nearby_places")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<List<NearbyPlace>>(v, (JsonSerializerOptions?)null)
            );

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

        builder.Property(p => p.FloorPlans)
            .HasColumnName("floor_plans")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<List<Document>>(v, (JsonSerializerOptions?)null)
            );

        builder.Property(p => p.VideoTourUrl)
            .HasColumnName("video_tour_url");

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
