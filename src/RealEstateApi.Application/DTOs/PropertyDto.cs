using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.DTOs;

/// <summary>
/// DTO for creating a new property
/// </summary>
public class CreatePropertyDto
{
    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string? Developer { get; set; }
    public string? Category { get; set; } // Development Project, Land Sale
    public string Type { get; set; } = string.Empty; // Apartment, Villa, Land, etc.
    public string? Status { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;

    // Property Mode
    public string? PropertyMode { get; set; } // "Standalone" or "MultiUnit"

    // Units (all properties have units array)
    public List<CreateUnitDto>? Units { get; set; }

    // Location
    public string Location { get; set; } = string.Empty; // General location
    public string? StreetAddress { get; set; }
    public string? Area { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Pricing
    public string? PriceRange { get; set; }

    // Project Details
    public string? CompletionDate { get; set; }
    public int? TotalUnits { get; set; }
    public int? TotalFloors { get; set; }
    public decimal? TotalLandArea { get; set; }
    public string? LandAreaUnit { get; set; } // sqft, sqm
    public List<string>? UnitTypesAvailable { get; set; }

    // Unit Specifications (Ranges/Summary)
    public string? BedroomsRange { get; set; }
    public string? BathroomsRange { get; set; }
    public string? AreaRange { get; set; }
    public string? FurnishingStatus { get; set; } // Furnished, Semi, Unfurnished

    // Descriptions
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    public List<string>? KeyHighlights { get; set; }

    // Features & Amenities
    public List<string> Features { get; set; } = new();
    public List<Amenity> Amenities { get; set; } = new();
    public List<Specification> Specifications { get; set; } = new();
    public List<NearbyPlace>? NearbyPlaces { get; set; }

    // Media
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = new();
    public List<Document>? FloorPlans { get; set; }
    public string? VideoTourUrl { get; set; }
}

/// <summary>
/// DTO for updating an existing property
/// </summary>
public class UpdatePropertyDto
{
    // Basic Information
    public string? Name { get; set; }
    public string? Developer { get; set; }
    public string? Category { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsFeatured { get; set; }

    // Property Mode
    public string? PropertyMode { get; set; } // "Standalone" or "MultiUnit"

    // Units (all properties have units array)
    public List<CreateUnitDto>? Units { get; set; }

    // Location
    public string? Location { get; set; }
    public string? StreetAddress { get; set; }
    public string? Area { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Pricing
    public string? PriceRange { get; set; }

    // Project Details
    public string? CompletionDate { get; set; }
    public int? TotalUnits { get; set; }
    public int? TotalFloors { get; set; }
    public decimal? TotalLandArea { get; set; }
    public string? LandAreaUnit { get; set; }
    public List<string>? UnitTypesAvailable { get; set; }

    // Unit Specifications
    public string? BedroomsRange { get; set; }
    public string? BathroomsRange { get; set; }
    public string? AreaRange { get; set; }
    public string? FurnishingStatus { get; set; }

    // Descriptions
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    public List<string>? KeyHighlights { get; set; }

    // Features & Amenities
    public List<string>? Features { get; set; }
    public List<Amenity>? Amenities { get; set; }
    public List<Specification>? Specifications { get; set; }
    public List<NearbyPlace>? NearbyPlaces { get; set; }

    // Media
    public string? MainImage { get; set; }
    public List<string>? Images { get; set; }
    public List<Document>? FloorPlans { get; set; }
    public string? VideoTourUrl { get; set; }
}

/// <summary>
/// DTO for property response (includes units)
/// </summary>
public class PropertyResponseDto
{
    public int Id { get; set; }
    public string AgentId { get; set; } = string.Empty;

    // Basic Information
    public string Name { get; set; } = string.Empty;
    public string? Developer { get; set; }
    public string? Category { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }

    // Property Mode
    public string PropertyMode { get; set; } = "Standalone";

    // Location
    public string Location { get; set; } = string.Empty;
    public string? StreetAddress { get; set; }
    public string? Area { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Pricing
    public string? PriceRange { get; set; }

    // Project Details
    public string? CompletionDate { get; set; }
    public int? TotalUnits { get; set; }
    public int? TotalFloors { get; set; }
    public decimal? TotalLandArea { get; set; }
    public string? LandAreaUnit { get; set; }
    public List<string>? UnitTypesAvailable { get; set; }

    // Unit Specifications
    public string? BedroomsRange { get; set; }
    public string? BathroomsRange { get; set; }
    public string? AreaRange { get; set; }
    public string? FurnishingStatus { get; set; }

    // Descriptions
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    public List<string>? KeyHighlights { get; set; }

    // Features & Amenities
    public List<string> Features { get; set; } = new();
    public List<Amenity> Amenities { get; set; } = new();
    public List<Specification> Specifications { get; set; } = new();
    public List<NearbyPlace>? NearbyPlaces { get; set; }

    // Media
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = new();
    public List<Document>? FloorPlans { get; set; }
    public string? VideoTourUrl { get; set; }

    // Units
    public List<UnitResponseDto> Units { get; set; } = new();

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new unit
/// </summary>
public class CreateUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal Area { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }

    // Media & Marketing
    public List<string>? Images { get; set; }
    public List<string>? FloorPlans { get; set; }
    public List<string>? Amenities { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an existing unit
/// </summary>
public class UpdateUnitDto
{
    public string? UnitNumber { get; set; }
    public string? Floor { get; set; }
    public string? Type { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? Area { get; set; }
    public decimal? Price { get; set; }
    public string? Status { get; set; }

    // Media & Marketing
    public List<string>? Images { get; set; }
    public List<string>? FloorPlans { get; set; }
    public List<string>? Amenities { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for unit response
/// </summary>
public class UnitResponseDto
{
    public string Id { get; set; } = string.Empty;
    public int PropertyId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal Area { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;

    // Media & Marketing
    public List<string> Images { get; set; } = new();
    public List<string> FloorPlans { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for paginated property response
/// </summary>
public class PagedPropertiesResponse
{
    public List<PropertyResponseDto> Projects { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}

// Note: NearbyPlace, Document, Amenity, and Specification are imported from RealEstateApi.Domain.Entities
// They are value objects shared between domain and application layers
