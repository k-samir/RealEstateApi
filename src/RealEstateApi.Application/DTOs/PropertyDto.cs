using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.DTOs;

/// <summary>
/// DTO for creating a new property
/// </summary>
public class CreatePropertyDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? CompletionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    public List<string> Features { get; set; } = new();
    public List<Amenity> Amenities { get; set; } = new();
    public List<Specification> Specifications { get; set; } = new();
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = new();
    public string? BedroomsRange { get; set; }
    public string? BathroomsRange { get; set; }
    public string? AreaRange { get; set; }
    public string? PriceRange { get; set; }
}

/// <summary>
/// DTO for updating an existing property
/// </summary>
public class UpdatePropertyDto
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? CompletionDate { get; set; }
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    public List<string>? Features { get; set; }
    public List<Amenity>? Amenities { get; set; }
    public List<Specification>? Specifications { get; set; }
    public string? MainImage { get; set; }
    public List<string>? Images { get; set; }
    public string? BedroomsRange { get; set; }
    public string? BathroomsRange { get; set; }
    public string? AreaRange { get; set; }
    public string? PriceRange { get; set; }
}

/// <summary>
/// DTO for property response (includes units)
/// </summary>
public class PropertyResponseDto
{
    public int Id { get; set; }
    public string AgentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CompletionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    public List<string> Features { get; set; } = new();
    public List<Amenity> Amenities { get; set; } = new();
    public List<Specification> Specifications { get; set; } = new();
    public string? MainImage { get; set; }
    public List<string> Images { get; set; } = new();
    public string? BedroomsRange { get; set; }
    public string? BathroomsRange { get; set; }
    public string? AreaRange { get; set; }
    public string? PriceRange { get; set; }
    public List<UnitResponseDto> Units { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
