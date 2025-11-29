namespace RealEstateApi.Domain.Entities;

/// <summary>
/// Property/Project entity - represents a real estate development project
/// Domain model - no infrastructure dependencies
/// </summary>
public class Property
{
    public int Id { get; set; }
    public string AgentId { get; set; } = string.Empty; // User ID from Better Auth

    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Type { get; set; } = string.Empty; // "Appartements & Villas", "Villa", etc.
    public string Status { get; set; } = "draft"; // "draft" | "published" | "sold" | "reserved"
    public string? CompletionDate { get; set; } // e.g., "2025", "Q3 2025"

    // Descriptions
    public string Description { get; set; } = string.Empty; // Short description
    public string? LongDescription { get; set; } // Detailed description

    // Features & Amenities (JSON arrays)
    public List<string> Features { get; set; } = new(); // ["Piscine", "Parking", ...]
    public List<Amenity> Amenities { get; set; } = new(); // [{Icon: "Waves", Label: "Piscine"}, ...]
    public List<Specification> Specifications { get; set; } = new(); // [{Label: "Surface", Value: "120m²"}, ...]

    // Media
    public string? MainImage { get; set; } // Main property image URL
    public List<string> Images { get; set; } = new(); // Array of image URLs

    // Summary Fields (ranges for display)
    public string? BedroomsRange { get; set; } // e.g., "2-5"
    public string? BathroomsRange { get; set; } // e.g., "2-4"
    public string? AreaRange { get; set; } // e.g., "120-350m²"
    public string? PriceRange { get; set; } // e.g., "À partir de 2,500,000 MAD"

    // Relationships
    public List<Unit> Units { get; set; } = new(); // Available units

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Value object for property amenities
/// </summary>
public class Amenity
{
    public string Icon { get; set; } = string.Empty; // Lucide icon name
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// Value object for property specifications
/// </summary>
public class Specification
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
