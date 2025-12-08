using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.Interfaces;

/// <summary>
/// Repository interface for UserPermissions
/// </summary>
public interface IUserPermissionsRepository
{
    Task<UserPermissions?> GetByUserIdAsync(string userId);
    Task<UserPermissions> CreateAsync(UserPermissions permissions);
    Task<UserPermissions> UpdateAsync(UserPermissions permissions);
    Task DeleteAsync(string userId);
    Task<bool> ExistsAsync(string userId);
}
