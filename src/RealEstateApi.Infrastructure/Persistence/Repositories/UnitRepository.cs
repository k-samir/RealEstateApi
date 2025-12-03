using Microsoft.EntityFrameworkCore;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Unit repository implementation (Adapter)
/// Implements IUnitRepository port using EF Core
/// </summary>
public class UnitRepository : IUnitRepository
{
    private readonly ApplicationDbContext _context;

    public UnitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Unit>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Units
            .Where(u => u.PropertyId == propertyId)
            .OrderBy(u => u.UnitNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<Unit?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Units
            .Include(u => u.Property)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Unit> CreateAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        // Domain entity already sets CreatedAt/UpdatedAt via factory method
        // EF Core will use field-based access configured in UnitConfiguration

        _context.Units.Add(unit);
        await _context.SaveChangesAsync(cancellationToken);

        return unit;
    }

    public async Task<Unit> UpdateAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        // Domain entity already updates UpdatedAt via domain methods
        // EF Core will use field-based access configured in UnitConfiguration

        _context.Units.Update(unit);
        await _context.SaveChangesAsync(cancellationToken);

        return unit;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var unit = await _context.Units.FindAsync(new object[] { id }, cancellationToken);
        if (unit != null)
        {
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
