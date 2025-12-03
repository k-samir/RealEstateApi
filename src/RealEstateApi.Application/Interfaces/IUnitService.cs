using RealEstateApi.Application.DTOs;

namespace RealEstateApi.Application.Interfaces;

/// <summary>
/// Unit service interface - defines unit management use cases
/// </summary>
public interface IUnitService
{
    Task<IEnumerable<UnitResponseDto>> GetUnitsByPropertyIdAsync(int propertyId);
    Task<UnitResponseDto> GetUnitByIdAsync(string id);
    Task<UnitResponseDto> CreateUnitAsync(int propertyId, CreateUnitDto dto, string userId);
    Task<UnitResponseDto> UpdateUnitAsync(string id, UpdateUnitDto dto, string userId, string? userRole);
    Task DeleteUnitAsync(string id, string userId, string? userRole);
}
