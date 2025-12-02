namespace RealEstateApi.Domain.Enums;

/// <summary>
/// Property mode enumeration
/// Distinguishes between standalone properties and multi-unit projects
/// </summary>
public enum PropertyMode
{
    /// <summary>
    /// Single unit property (villa, apartment, land) - unit details stored in Property entity
    /// </summary>
    Standalone,

    /// <summary>
    /// Multi-unit project (development with multiple units) - uses Units table
    /// </summary>
    MultiUnit
}
