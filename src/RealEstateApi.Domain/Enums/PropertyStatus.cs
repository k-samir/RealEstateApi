namespace RealEstateApi.Domain.Enums;

/// <summary>
/// Property status enumeration - represents business lifecycle
/// Separate from IsPublished which controls online visibility
/// </summary>
public enum PropertyStatus
{
    Draft,
    Active,
    Sold,
    Rented,
    UnderConstruction,
    Completed,
    Archived
}
