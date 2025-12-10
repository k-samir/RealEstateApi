using Microsoft.EntityFrameworkCore;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for UserPermissions
/// </summary>
public class UserPermissionsRepository : IUserPermissionsRepository
{
    private readonly ApplicationDbContext _context;

    public UserPermissionsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserPermissions?> GetByUserIdAsync(string userId)
    {
        return await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<UserPermissions> CreateAsync(UserPermissions permissions)
    {
        _context.UserPermissions.Add(permissions);
        await _context.SaveChangesAsync();
        return permissions;
    }

    public async Task<UserPermissions> UpdateAsync(UserPermissions permissions)
    {
        _context.UserPermissions.Update(permissions);
        await _context.SaveChangesAsync();
        return permissions;
    }

    public async Task DeleteAsync(string userId)
    {
        var permissions = await GetByUserIdAsync(userId);
        if (permissions != null)
        {
            _context.UserPermissions.Remove(permissions);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string userId)
    {
        return await _context.UserPermissions
            .AnyAsync(up => up.UserId == userId);
    }
}
