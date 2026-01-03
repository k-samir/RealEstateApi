using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.Interfaces;

/// <summary>
/// Port (interface) for Property repository
/// Hexagonal Architecture: Application defines the port, Infrastructure implements it
/// </summary>
public interface IPropertyRepository
{
    // Query operations
    Task<(List<Property> items, int total)> GetAllAsync(PropertyFilter? filter = null, CancellationToken cancellationToken = default);
    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Property>> GetByAgentIdAsync(string agentId, CancellationToken cancellationToken = default);

    // Command operations
    Task<Property> CreateAsync(Property property, CancellationToken cancellationToken = default);
    Task<Property> UpdateAsync(Property property, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    // Check ownership (for authorization)
    Task<bool> IsOwnerAsync(int propertyId, string agentId, CancellationToken cancellationToken = default);

    // Get distinct cities
    Task<IEnumerable<string>> GetUniqueCitiesAsync(CancellationToken cancellationToken = default);

    // Get cities with their associated areas
    Task<CitiesWithAreasDto> GetCitiesWithAreasAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Filter DTO for property queries
/// </summary>
public class PropertyFilter
{
    public string? City { get; set; }
    public string? Location { get; set; }  // Neighborhood filter
    public string? Type { get; set; }
    public string? Status { get; set; }
    public int? MinBedrooms { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchQuery { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO for cities with their associated areas
/// </summary>
public class CitiesWithAreasDto
{
    public List<string> Cities { get; set; } = new();
    public Dictionary<string, List<string>> CityAreaMapping { get; set; } = new();
}
