using RealEstateApi.Application.DTOs;

namespace RealEstateApi.Application.Interfaces;

/// <summary>
/// Property service interface - defines use cases for property management
/// </summary>
public interface IPropertyService
{
    Task<PropertyResponseDto> GetPropertyByIdAsync(int id);
    Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync(PropertyFilter filter);
    Task<IEnumerable<PropertyResponseDto>> GetAgentPropertiesAsync(string agentId);
    Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto dto, string agentId);
    Task<PropertyResponseDto> UpdatePropertyAsync(int id, UpdatePropertyDto dto, string userId, string? userRole);
    Task DeletePropertyAsync(int id, string userId, string? userRole);
    Task<PropertyResponseDto> PublishPropertyAsync(int id, string userId);
}
