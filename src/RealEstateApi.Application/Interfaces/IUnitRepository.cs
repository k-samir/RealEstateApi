using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.Interfaces;

/// <summary>
/// Port (interface) for Unit repository
/// </summary>
public interface IUnitRepository
{
    Task<IEnumerable<Unit>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<Unit?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Unit> CreateAsync(Unit unit, CancellationToken cancellationToken = default);
    Task<Unit> UpdateAsync(Unit unit, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
