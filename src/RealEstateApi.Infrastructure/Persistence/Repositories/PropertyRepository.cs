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

    public async Task<(List<Property> items, int total)> GetAllAsync(PropertyFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Properties
            .Include(p => p.Units)
            .AsQueryable();

        // IMPORTANT: Filter by IsPublished for public listings
        // Only show published properties to public users
        query = query.Where(p => p.IsPublished);

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

            // Filter by MinBedrooms - parse BedroomsRange string (e.g., "2-5" or "3+")
            if (filter.MinBedrooms.HasValue)
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.BedroomsRange) && 
                    ParseMinBedroomsFromRange(p.BedroomsRange) >= filter.MinBedrooms.Value);
            }

            // Filter by MinPrice and MaxPrice - parse PriceRange string (e.g., "À partir de 2,500,000 MAD" or "1,000,000 - 3,000,000 MAD")
            if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.PriceRange) && 
                    IsPriceInRange(p.PriceRange, filter.MinPrice, filter.MaxPrice));
            }

            // Get total count BEFORE applying pagination
            var total = await query.CountAsync(cancellationToken);

            // Apply pagination
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        // No filter - return all
        var totalCount = await query.CountAsync(cancellationToken);
        var allItems = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return (allItems, totalCount);
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

    // Helper methods for parsing range strings

    /// <summary>
    /// Parse minimum bedrooms from BedroomsRange string (e.g., "2-5" returns 2, "3+" returns 3)
    /// </summary>
    private static int ParseMinBedroomsFromRange(string bedroomsRange)
    {
        if (string.IsNullOrWhiteSpace(bedroomsRange))
            return 0;

        // Handle "3+" format
        if (bedroomsRange.Contains("+"))
        {
            var numberPart = bedroomsRange.Replace("+", "").Trim();
            return int.TryParse(numberPart, out var result) ? result : 0;
        }

        // Handle "2-5" format
        if (bedroomsRange.Contains("-"))
        {
            var parts = bedroomsRange.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[0].Trim(), out var min))
                return min;
        }

        // Try to parse as single number
        return int.TryParse(bedroomsRange.Trim(), out var singleValue) ? singleValue : 0;
    }

    /// <summary>
    /// Check if price range matches the filter criteria
    /// Parses strings like "À partir de 2,500,000 MAD" or "1,000,000 - 3,000,000 MAD"
    /// </summary>
    private static bool IsPriceInRange(string priceRange, decimal? minPrice, decimal? maxPrice)
    {
        if (string.IsNullOrWhiteSpace(priceRange))
            return false;

        // Remove common French phrases and currency
        var cleanedRange = priceRange
            .Replace("À partir de", "")
            .Replace("à partir de", "")
            .Replace("MAD", "")
            .Replace("DH", "")
            .Replace(",", "")
            .Replace(" ", "")
            .Trim();

        // Handle "À partir de X" format (minimum price only)
        if (priceRange.Contains("partir de", StringComparison.OrdinalIgnoreCase))
        {
            if (decimal.TryParse(cleanedRange, out var propertyMinPrice))
            {
                // Property has a minimum price
                if (maxPrice.HasValue && propertyMinPrice > maxPrice.Value)
                    return false; // Property min is too high

                if (minPrice.HasValue && propertyMinPrice < minPrice.Value)
                    return false; // Property min is too low

                return true;
            }
        }

        // Handle "X - Y" format (price range)
        if (cleanedRange.Contains("-"))
        {
            var parts = cleanedRange.Split('-');
            if (parts.Length == 2 &&
                decimal.TryParse(parts[0].Trim(), out var propertyMinPrice) &&
                decimal.TryParse(parts[1].Trim(), out var propertyMaxPrice))
            {
                // Check if ranges overlap
                if (minPrice.HasValue && propertyMaxPrice < minPrice.Value)
                    return false; // Property range is below filter min

                if (maxPrice.HasValue && propertyMinPrice > maxPrice.Value)
                    return false; // Property range is above filter max

                return true;
            }
        }

        // Try to parse as single price
        if (decimal.TryParse(cleanedRange, out var singlePrice))
        {
            if (minPrice.HasValue && singlePrice < minPrice.Value)
                return false;

            if (maxPrice.HasValue && singlePrice > maxPrice.Value)
                return false;

            return true;
        }

        // If we can't parse it, include it (don't filter out)
        return true;
    }
}
