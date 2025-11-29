using Microsoft.EntityFrameworkCore;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;

namespace RealEstateApi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Property repository implementation (Adapter)
/// Implements IPropertyRepository port using EF Core
/// </summary>
public class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Property>> GetAllAsync(PropertyFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Properties
            .Include(p => p.Units)
            .AsQueryable();

        // Apply filters if provided
        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(p => p.Location.Contains(filter.Location));
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(p => p.Type == filter.Type);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                // Parse string status to enum
                if (Enum.TryParse<PropertyStatus>(filter.Status, ignoreCase: true, out var statusEnum))
                {
                    query = query.Where(p => p.Status == statusEnum);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(filter.SearchQuery) ||
                    p.Description.Contains(filter.SearchQuery) ||
                    p.Location.Contains(filter.SearchQuery)
                );
            }

            // Pagination
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Units)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Property>> GetByAgentIdAsync(string agentId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Include(p => p.Units)
            .Where(p => p.AgentId == agentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Property> CreateAsync(Property property, CancellationToken cancellationToken = default)
    {
        // Domain entity already sets CreatedAt/UpdatedAt via factory method
        // EF Core will use field-based access configured in PropertyConfiguration

        _context.Properties.Add(property);
        await _context.SaveChangesAsync(cancellationToken);

        return property;
    }

    public async Task<Property> UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        // Domain entity already updates UpdatedAt via domain methods
        // EF Core will use field-based access configured in PropertyConfiguration

        _context.Properties.Update(property);
        await _context.SaveChangesAsync(cancellationToken);

        return property;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var property = await _context.Properties.FindAsync(new object[] { id }, cancellationToken);
        if (property != null)
        {
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> IsOwnerAsync(int propertyId, string agentId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .AnyAsync(p => p.Id == propertyId && p.AgentId == agentId, cancellationToken);
    }
}
