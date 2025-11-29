using RealEstateApi.Domain.Enums;
using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

/// <summary>
/// Property/Project entity - represents a real estate development project
/// Rich domain model with business logic and invariants
/// </summary>
public class Property
{
    // Private setters - only domain methods can modify state
    public int Id { get; private set; }
    public string AgentId { get; private set; } = string.Empty;

    // Basic Information
    public string Name { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public PropertyStatus Status { get; private set; } = PropertyStatus.Draft;
    public string? CompletionDate { get; private set; }

    // Descriptions
    public string Description { get; private set; } = string.Empty;
    public string? LongDescription { get; private set; }

    // Features & Amenities (JSON arrays)
    public List<string> Features { get; private set; } = new();
    public List<Amenity> Amenities { get; private set; } = new();
    public List<Specification> Specifications { get; private set; } = new();

    // Media
    public string? MainImage { get; private set; }
    public List<string> Images { get; private set; } = new();

    // Summary Fields (ranges for display)
    public string? BedroomsRange { get; private set; }
    public string? BathroomsRange { get; private set; }
    public string? AreaRange { get; private set; }
    public string? PriceRange { get; private set; }

    // Relationships
    private readonly List<Unit> _units = new();
    public IReadOnlyCollection<Unit> Units => _units.AsReadOnly();

    // Metadata
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor - use factory method
    private Property() { }

    /// <summary>
    /// Factory method to create a new property
    /// Ensures all required invariants are met
    /// </summary>
    public static Property Create(
        string agentId,
        string name,
        string location,
        string type,
        string description)
    {
        if (string.IsNullOrWhiteSpace(agentId))
            throw new DomainException("Property must have an agent");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Property name is required");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Property location is required");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Property type is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Property description is required");

        var now = DateTime.UtcNow;

        return new Property
        {
            AgentId = agentId,
            Name = name,
            Location = location,
            Type = type,
            Description = description,
            Status = PropertyStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Update basic property details
    /// </summary>
    public void UpdateDetails(
        string name,
        string location,
        string type,
        string description,
        string? longDescription = null,
        decimal? latitude = null,
        decimal? longitude = null,
        string? completionDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Property name is required");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Property location is required");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Property type is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Property description is required");

        Name = name;
        Location = location;
        Type = type;
        Description = description;
        LongDescription = longDescription;
        Latitude = latitude;
        Longitude = longitude;
        CompletionDate = completionDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update property features and amenities
    /// </summary>
    public void UpdateFeaturesAndAmenities(
        List<string>? features = null,
        List<Amenity>? amenities = null,
        List<Specification>? specifications = null)
    {
        if (features != null)
            Features = features;

        if (amenities != null)
            Amenities = amenities;

        if (specifications != null)
            Specifications = specifications;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update property images
    /// </summary>
    public void UpdateImages(string? mainImage = null, List<string>? images = null)
    {
        if (mainImage != null)
            MainImage = mainImage;

        if (images != null)
            Images = images;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update property summary ranges
    /// </summary>
    public void UpdateRanges(
        string? bedroomsRange = null,
        string? bathroomsRange = null,
        string? areaRange = null,
        string? priceRange = null)
    {
        if (bedroomsRange != null)
            BedroomsRange = bedroomsRange;

        if (bathroomsRange != null)
            BathroomsRange = bathroomsRange;

        if (areaRange != null)
            AreaRange = areaRange;

        if (priceRange != null)
            PriceRange = priceRange;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Publish the property
    /// Business rule: Property must have required fields to be published
    /// </summary>
    public void Publish()
    {
        if (!CanBePublished())
            throw new DomainException(
                "Property cannot be published. Required: name, description, main image, and at least one unit.");

        Status = PropertyStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark property as reserved
    /// </summary>
    public void Reserve()
    {
        if (Status != PropertyStatus.Published)
            throw new DomainException("Only published properties can be reserved");

        Status = PropertyStatus.Reserved;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark property as sold
    /// </summary>
    public void MarkAsSold()
    {
        if (Status != PropertyStatus.Published && Status != PropertyStatus.Reserved)
            throw new DomainException("Only published or reserved properties can be marked as sold");

        Status = PropertyStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revert to draft status
    /// </summary>
    public void RevertToDraft()
    {
        if (Status == PropertyStatus.Sold)
            throw new DomainException("Sold properties cannot be reverted to draft");

        Status = PropertyStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if property can be published
    /// Business rules for publication
    /// </summary>
    public bool CanBePublished()
    {
        return !string.IsNullOrEmpty(Name)
            && !string.IsNullOrEmpty(Description)
            && !string.IsNullOrEmpty(MainImage)
            && _units.Count > 0;
    }

    /// <summary>
    /// Check if user owns this property
    /// </summary>
    public bool IsOwnedBy(string userId)
    {
        return AgentId == userId;
    }

    /// <summary>
    /// Add a unit to the property
    /// </summary>
    public void AddUnit(Unit unit)
    {
        if (unit == null)
            throw new DomainException("Unit cannot be null");

        _units.Add(unit);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Remove a unit from the property
    /// Business rule: Cannot remove units from sold properties
    /// </summary>
    public void RemoveUnit(Unit unit)
    {
        if (unit == null)
            throw new DomainException("Unit cannot be null");

        if (Status == PropertyStatus.Sold)
            throw new DomainException("Cannot remove units from sold properties");

        _units.Remove(unit);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if property is editable
    /// Business rule: Sold properties cannot be edited
    /// </summary>
    public bool IsEditable()
    {
        return Status != PropertyStatus.Sold;
    }

    // Internal setter for EF Core (needed for loading from database)
    internal void SetId(int id) => Id = id;
}

/// <summary>
/// Value object for property amenities
/// </summary>
public class Amenity
{
    public string Icon { get; set; } = string.Empty;
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
