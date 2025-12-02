using RealEstateApi.Domain.Enums;
using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

/// <summary>
/// Unit entity - represents an individual unit within a property
/// Rich domain model with business logic
/// </summary>
public class Unit
{
    // Private setters - only domain methods can modify state
    public string Id { get; private set; } = string.Empty;
    public int PropertyId { get; private set; }

    // Unit Details
    public string UnitNumber { get; private set; } = string.Empty;
    public string? Floor { get; private set; }
    public string Type { get; private set; } = string.Empty;

    // Specifications
    public int Bedrooms { get; private set; }
    public int Bathrooms { get; private set; }
    public decimal Area { get; private set; }

    // Pricing & Availability
    public decimal Price { get; private set; }
    public UnitStatus Status { get; private set; } = UnitStatus.Available;

    // Media & Marketing
    public List<string> Images { get; private set; } = new();
    public List<string> FloorPlans { get; private set; } = new();
    public List<string> Amenities { get; private set; } = new();
    public string? Description { get; private set; }

    // Relationships
    public Property Property { get; private set; } = null!;

    // Metadata
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor - use factory method
    private Unit() { }

    /// <summary>
    /// Factory method to create a new unit
    /// </summary>
    public static Unit Create(
        int propertyId,
        string unitNumber,
        string type,
        int bedrooms,
        int bathrooms,
        decimal area,
        decimal price,
        string? floor = null,
        List<string>? images = null,
        List<string>? floorPlans = null,
        List<string>? amenities = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new DomainException("Unit number is required");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Unit type is required");

        if (bedrooms < 0)
            throw new DomainException("Bedrooms cannot be negative");

        if (bathrooms < 0)
            throw new DomainException("Bathrooms cannot be negative");

        if (area <= 0)
            throw new DomainException("Area must be greater than zero");

        if (price < 0)
            throw new DomainException("Price cannot be negative");

        var now = DateTime.UtcNow;

        return new Unit
        {
            Id = Guid.NewGuid().ToString(),
            PropertyId = propertyId,
            UnitNumber = unitNumber,
            Type = type,
            Bedrooms = bedrooms,
            Bathrooms = bathrooms,
            Area = area,
            Price = price,
            Floor = floor,
            Images = images ?? new List<string>(),
            FloorPlans = floorPlans ?? new List<string>(),
            Amenities = amenities ?? new List<string>(),
            Description = description,
            Status = UnitStatus.Available,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Update unit details
    /// </summary>
    public void UpdateDetails(
        string unitNumber,
        string type,
        int bedrooms,
        int bathrooms,
        decimal area,
        decimal price,
        string? floor = null)
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Cannot update sold units");

        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new DomainException("Unit number is required");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Unit type is required");

        if (bedrooms < 0)
            throw new DomainException("Bedrooms cannot be negative");

        if (bathrooms < 0)
            throw new DomainException("Bathrooms cannot be negative");

        if (area <= 0)
            throw new DomainException("Area must be greater than zero");

        if (price < 0)
            throw new DomainException("Price cannot be negative");

        UnitNumber = unitNumber;
        Type = type;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        Area = area;
        Price = price;
        Floor = floor;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reserve the unit
    /// </summary>
    public void Reserve()
    {
        if (Status != UnitStatus.Available)
            throw new DomainException("Only available units can be reserved");

        Status = UnitStatus.Reserved;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark unit as sold
    /// </summary>
    public void MarkAsSold()
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Unit is already sold");

        Status = UnitStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Make unit available again
    /// </summary>
    public void MakeAvailable()
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Sold units cannot be made available again");

        Status = UnitStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update price
    /// </summary>
    public void UpdatePrice(decimal newPrice)
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Cannot update price of sold units");

        if (newPrice < 0)
            throw new DomainException("Price cannot be negative");

        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if unit is available for purchase
    /// </summary>
    public bool IsAvailable()
    {
        return Status == UnitStatus.Available;
    }

    /// <summary>
    /// Update unit media (images and floor plans)
    /// </summary>
    public void UpdateMedia(List<string>? images = null, List<string>? floorPlans = null)
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Cannot update media of sold units");

        if (images != null)
            Images = images;

        if (floorPlans != null)
            FloorPlans = floorPlans;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update unit amenities
    /// </summary>
    public void UpdateAmenities(List<string> amenities)
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Cannot update amenities of sold units");

        Amenities = amenities ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update unit description
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (Status == UnitStatus.Sold)
            throw new DomainException("Cannot update description of sold units");

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
