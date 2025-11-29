namespace RealEstateApi.Domain.Entities;

/// <summary>
/// Unit entity - represents an individual unit within a property
/// (apartment, villa, duplex, penthouse, etc.)
/// Domain model - no infrastructure dependencies
/// </summary>
public class Unit
{
    public string Id { get; set; } = string.Empty; // e.g., "u1", "u2"
    public int PropertyId { get; set; }

    // Unit Details
    public string UnitNumber { get; set; } = string.Empty; // e.g., "A12", "V01", "D05"
    public string? Floor { get; set; } // e.g., "RDC", "1er", "2ème"
    public string Type { get; set; } = string.Empty; // "Appartement", "Villa", "Duplex", "Penthouse"

    // Specifications
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal Area { get; set; } // Square meters

    // Pricing & Availability
    public decimal Price { get; set; } // Price in MAD
    public string Status { get; set; } = "Available"; // "Available" | "Reserved" | "Sold"

    // Relationships
    public Property Property { get; set; } = null!;

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
