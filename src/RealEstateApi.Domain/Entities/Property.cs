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
    public string? Developer { get; private set; }
    public string? Category { get; private set; } // Development Project, Land Sale
    public string Type { get; private set; } = string.Empty;
    public PropertyStatus Status { get; private set; } = PropertyStatus.Draft;
    public bool IsPublished { get; private set; } = false;
    public bool IsFeatured { get; private set; } = false;

    // Property Mode
    public PropertyMode Mode { get; private set; } = PropertyMode.Standalone;

    // Standalone Unit Fields (used when Mode = Standalone)
    public int? Bedrooms { get; private set; }
    public int? Bathrooms { get; private set; }
    public decimal? UnitArea { get; private set; }
    public decimal? UnitPrice { get; private set; }
    public string? AreaUnit { get; private set; }

    // Location
    public string Location { get; private set; } = string.Empty; // General location
    public string? StreetAddress { get; private set; }
    public string? Area { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }

    // Pricing
    public string? PriceRange { get; private set; }

    // Project Details
    public string? CompletionDate { get; private set; }
    public int? TotalUnits { get; private set; }
    public int? TotalFloors { get; private set; }
    public decimal? TotalLandArea { get; private set; }
    public string? LandAreaUnit { get; private set; }
    public List<string>? UnitTypesAvailable { get; private set; }

    // Unit Specifications (ranges for display)
    public string? BedroomsRange { get; private set; }
    public string? BathroomsRange { get; private set; }
    public string? AreaRange { get; private set; }
    public string? FurnishingStatus { get; private set; }

    // Descriptions
    public string Description { get; private set; } = string.Empty;
    public string? LongDescription { get; private set; }
    public List<string>? KeyHighlights { get; private set; }

    // Features & Amenities (JSON arrays)
    public List<string> Features { get; private set; } = new();
    public List<Amenity> Amenities { get; private set; } = new();
    public List<Specification> Specifications { get; private set; } = new();
    public List<NearbyPlace>? NearbyPlaces { get; private set; }

    // Media
    public string? MainImage { get; private set; }
    public List<string> Images { get; private set; } = new();
    public List<Document>? FloorPlans { get; private set; }
    public string? VideoTourUrl { get; private set; }

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
    public void UpdateBasicInfo(
        string name,
        string type,
        string description,
        string? developer = null,
        string? category = null,
        bool? isFeatured = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Property name is required");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Property type is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Property description is required");

        Name = name;
        Type = type;
        Description = description;
        Developer = developer;
        Category = category;

        if (isFeatured.HasValue)
            IsFeatured = isFeatured.Value;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update location details
    /// </summary>
    public void UpdateLocation(
        string location,
        string? streetAddress = null,
        string? area = null,
        string? city = null,
        string? state = null,
        string? country = null,
        string? postalCode = null,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Property location is required");

        Location = location;
        StreetAddress = streetAddress;
        Area = area;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        Latitude = latitude;
        Longitude = longitude;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update project details
    /// </summary>
    public void UpdateProjectDetails(
        string? priceRange = null,
        string? completionDate = null,
        int? totalUnits = null,
        int? totalFloors = null,
        decimal? totalLandArea = null,
        string? landAreaUnit = null,
        List<string>? unitTypesAvailable = null)
    {
        PriceRange = priceRange;
        CompletionDate = completionDate;
        TotalUnits = totalUnits;
        TotalFloors = totalFloors;
        TotalLandArea = totalLandArea;
        LandAreaUnit = landAreaUnit;
        UnitTypesAvailable = unitTypesAvailable;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update descriptions
    /// </summary>
    public void UpdateDescriptions(
        string? description = null,
        string? longDescription = null,
        List<string>? keyHighlights = null)
    {
        if (description != null)
            Description = description;

        LongDescription = longDescription;
        KeyHighlights = keyHighlights;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update property features and amenities
    /// </summary>
    public void UpdateFeaturesAndAmenities(
        List<string>? features = null,
        List<Amenity>? amenities = null,
        List<Specification>? specifications = null,
        List<NearbyPlace>? nearbyPlaces = null)
    {
        if (features != null)
            Features = features;

        if (amenities != null)
            Amenities = amenities;

        if (specifications != null)
            Specifications = specifications;

        if (nearbyPlaces != null)
            NearbyPlaces = nearbyPlaces;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update property media
    /// </summary>
    public void UpdateMedia(
        string? mainImage = null,
        List<string>? images = null,
        List<Document>? floorPlans = null,
        string? videoTourUrl = null)
    {
        if (mainImage != null)
            MainImage = mainImage;

        if (images != null)
            Images = images;

        if (floorPlans != null)
            FloorPlans = floorPlans;

        if (videoTourUrl != null)
            VideoTourUrl = videoTourUrl;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update unit specifications and ranges
    /// </summary>
    public void UpdateUnitSpecifications(
        string? bedroomsRange = null,
        string? bathroomsRange = null,
        string? areaRange = null,
        string? furnishingStatus = null)
    {
        if (bedroomsRange != null)
            BedroomsRange = bedroomsRange;

        if (bathroomsRange != null)
            BathroomsRange = bathroomsRange;

        if (areaRange != null)
            AreaRange = areaRange;

        if (furnishingStatus != null)
            FurnishingStatus = furnishingStatus;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Publish the property (make visible online)
    /// Business rule: Property must have required fields to be published
    /// </summary>
    public void Publish()
    {
        if (!CanBePublished())
            throw new DomainException(
                "Property cannot be published. Required: name, description, and mode-specific requirements.");

        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Unpublish the property (hide from public view)
    /// </summary>
    public void Unpublish()
    {
        IsPublished = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Set publish status directly (controls online visibility)
    /// </summary>
    public void SetPublishStatus(bool isPublished)
    {
        IsPublished = isPublished;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update the business status of the property
    /// </summary>
    public void UpdateStatus(PropertyStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark property as sold
    /// </summary>
    public void MarkAsSold()
    {
        Status = PropertyStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark property as rented
    /// </summary>
    public void MarkAsRented()
    {
        Status = PropertyStatus.Rented;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Archive the property
    /// </summary>
    public void Archive()
    {
        Status = PropertyStatus.Archived;
        IsPublished = false; // Archived properties should not be visible online
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if property can be published
    /// Business rules for publication based on property mode
    /// </summary>
    public bool CanBePublished()
    {
        // Mode-specific validation
        if (Mode == PropertyMode.Standalone)
        {
            // Standalone properties need unit details in property fields
            return Bedrooms.HasValue && Bathrooms.HasValue && UnitArea.HasValue && UnitPrice.HasValue;
        }
        else // MultiUnit
        {
            // Multi-unit properties need at least one unit
            return _units.Any();
        }
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
    /// Update standalone unit details (for Standalone mode properties)
    /// </summary>
    public void UpdateStandaloneUnitDetails(
        int bedrooms,
        int bathrooms,
        decimal area,
        decimal price,
        string? areaUnit = null)
    {
        if (Mode != PropertyMode.Standalone)
            throw new DomainException("Can only update standalone unit details for Standalone mode properties");

        if (bedrooms < 0)
            throw new DomainException("Bedrooms cannot be negative");

        if (bathrooms < 0)
            throw new DomainException("Bathrooms cannot be negative");

        if (area <= 0)
            throw new DomainException("Area must be greater than zero");

        if (price < 0)
            throw new DomainException("Price cannot be negative");

        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        UnitArea = area;
        UnitPrice = price;
        AreaUnit = areaUnit;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Set property mode (allows Standalone to MultiUnit conversion)
    /// </summary>
    public void SetMode(PropertyMode mode)
    {
        // Allow Standalone -> MultiUnit conversion
        if (Mode == PropertyMode.Standalone && mode == PropertyMode.MultiUnit)
        {
            Mode = mode;
            // Clear standalone unit fields when converting to MultiUnit
            Bedrooms = null;
            Bathrooms = null;
            UnitArea = null;
            UnitPrice = null;
            AreaUnit = null;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        // Don't allow MultiUnit -> Standalone if units exist
        if (Mode == PropertyMode.MultiUnit && mode == PropertyMode.Standalone)
        {
            if (_units.Any())
                throw new DomainException("Cannot convert to Standalone mode while units exist. Remove all units first.");

            Mode = mode;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        // Allow setting the same mode
        if (Mode == mode)
            return;

        throw new DomainException($"Invalid mode conversion from {Mode} to {mode}");
    }

    /// <summary>
    /// Calculate and update range fields from actual units (for MultiUnit properties)
    /// </summary>
    public void CalculateRangesFromUnits()
    {
        if (Mode != PropertyMode.MultiUnit)
            throw new DomainException("Can only calculate ranges for MultiUnit properties");

        if (!_units.Any())
        {
            // Clear ranges if no units
            BedroomsRange = null;
            BathroomsRange = null;
            AreaRange = null;
            PriceRange = null;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        // Calculate bedroom range
        var minBedrooms = _units.Min(u => u.Bedrooms);
        var maxBedrooms = _units.Max(u => u.Bedrooms);
        BedroomsRange = minBedrooms == maxBedrooms ? $"{minBedrooms}" : $"{minBedrooms}-{maxBedrooms}";

        // Calculate bathroom range
        var minBathrooms = _units.Min(u => u.Bathrooms);
        var maxBathrooms = _units.Max(u => u.Bathrooms);
        BathroomsRange = minBathrooms == maxBathrooms ? $"{minBathrooms}" : $"{minBathrooms}-{maxBathrooms}";

        // Calculate area range
        var minArea = _units.Min(u => u.Area);
        var maxArea = _units.Max(u => u.Area);
        AreaRange = minArea == maxArea ? $"{minArea:F0}" : $"{minArea:F0}-{maxArea:F0}";

        // Calculate price range
        var minPrice = _units.Min(u => u.Price);
        var maxPrice = _units.Max(u => u.Price);
        PriceRange = minPrice == maxPrice ? $"{minPrice:F0}" : $"{minPrice:F0}-{maxPrice:F0}";

        UpdatedAt = DateTime.UtcNow;
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

/// <summary>
/// Value object for nearby places
/// </summary>
public class NearbyPlace
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // School, Mall, Metro, etc.
    public string Distance { get; set; } = string.Empty; // "5 min walk", "2 km"
    public string? Icon { get; set; }
}

/// <summary>
/// Value object for documents (floor plans, brochures)
/// </summary>
public class Document
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // PDF, Image
    public long? Size { get; set; } // File size in bytes
}
