using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Exceptions;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;

namespace RealEstateApi.Application.Services;

/// <summary>
/// Unit service - implements unit management use cases
/// Orchestrates domain logic and repository operations
/// </summary>
public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyRepository _propertyRepository;

    public UnitService(
        IUnitRepository unitRepository,
        IPropertyRepository propertyRepository)
    {
        _unitRepository = unitRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<UnitResponseDto>> GetUnitsByPropertyIdAsync(int propertyId)
    {
        var units = await _unitRepository.GetByPropertyIdAsync(propertyId);
        return units.Select(MapToDto);
    }

    public async Task<UnitResponseDto> GetUnitByIdAsync(string id)
    {
        var unit = await _unitRepository.GetByIdAsync(id);

        if (unit == null)
            throw new NotFoundException($"Unit with ID {id} not found");

        return MapToDto(unit);
    }

    public async Task<UnitResponseDto> CreateUnitAsync(int propertyId, CreateUnitDto dto, string userId)
    {
        // Get the property first
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        if (property == null)
            throw new NotFoundException($"Property with ID {propertyId} not found");

        // Check ownership
        if (!property.IsOwnedBy(userId))
            throw new ForbiddenException("You don't have permission to add units to this property");

        // Validate property is in MultiUnit mode
        if (property.Mode != PropertyMode.MultiUnit)
            throw new BadRequestException("Can only add units to MultiUnit properties");

        // Parse status if provided
        UnitStatus? status = null;
        if (!string.IsNullOrEmpty(dto.Status))
        {
            status = Enum.Parse<UnitStatus>(dto.Status, ignoreCase: true);
        }

        // Use domain factory method - enforces business rules
        var unit = Unit.Create(
            propertyId,
            dto.UnitNumber,
            dto.Type,
            dto.Bedrooms,
            dto.Bathrooms,
            dto.Area,
            dto.Price,
            dto.Floor,
            dto.Images,
            dto.FloorPlans,
            dto.Amenities,
            dto.Description
        );

        // Set status if provided
        if (status.HasValue && status.Value != UnitStatus.Available)
        {
            if (status.Value == UnitStatus.Reserved)
                unit.Reserve();
            else if (status.Value == UnitStatus.Sold)
                unit.MarkAsSold();
        }

        var created = await _unitRepository.CreateAsync(unit);

        // Auto-calculate ranges from units after creating
        property.CalculateRangesFromUnits();
        await _propertyRepository.UpdateAsync(property);

        return MapToDto(created);
    }

    public async Task<UnitResponseDto> UpdateUnitAsync(string id, UpdateUnitDto dto, string userId, string? userRole)
    {
        var unit = await _unitRepository.GetByIdAsync(id);

        if (unit == null)
            throw new NotFoundException($"Unit with ID {id} not found");

        // Get the property to check authorization
        var property = await _propertyRepository.GetByIdAsync(unit.PropertyId);

        if (property == null)
            throw new NotFoundException($"Property with ID {unit.PropertyId} not found");

        // Check authorization
        if (!CanUserModifyUnit(property, userId, userRole))
            throw new ForbiddenException("You don't have permission to update this unit");

        // Update basic details if provided
        if (dto.UnitNumber != null || dto.Type != null || dto.Bedrooms.HasValue ||
            dto.Bathrooms.HasValue || dto.Area.HasValue || dto.Price.HasValue || dto.Floor != null)
        {
            unit.UpdateDetails(
                dto.UnitNumber ?? unit.UnitNumber,
                dto.Type ?? unit.Type,
                dto.Bedrooms ?? unit.Bedrooms,
                dto.Bathrooms ?? unit.Bathrooms,
                dto.Area ?? unit.Area,
                dto.Price ?? unit.Price,
                dto.Floor ?? unit.Floor
            );
        }

        // Update media if provided
        if (dto.Images != null || dto.FloorPlans != null)
        {
            unit.UpdateMedia(dto.Images, dto.FloorPlans);
        }

        // Update amenities if provided
        if (dto.Amenities != null)
        {
            unit.UpdateAmenities(dto.Amenities);
        }

        // Update description if provided
        if (dto.Description != null)
        {
            unit.UpdateDescription(dto.Description);
        }

        // Handle status changes
        if (!string.IsNullOrEmpty(dto.Status))
        {
            UpdateUnitStatus(unit, dto.Status);
        }

        var updated = await _unitRepository.UpdateAsync(unit);

        // Auto-calculate ranges from units after updating
        property.CalculateRangesFromUnits();
        await _propertyRepository.UpdateAsync(property);

        return MapToDto(updated);
    }

    public async Task DeleteUnitAsync(string id, string userId, string? userRole)
    {
        var unit = await _unitRepository.GetByIdAsync(id);

        if (unit == null)
            throw new NotFoundException($"Unit with ID {id} not found");

        // Get the property to check authorization
        var property = await _propertyRepository.GetByIdAsync(unit.PropertyId);

        if (property == null)
            throw new NotFoundException($"Property with ID {unit.PropertyId} not found");

        // Check authorization
        if (!CanUserModifyUnit(property, userId, userRole))
            throw new ForbiddenException("You don't have permission to delete this unit");

        await _unitRepository.DeleteAsync(id);

        // Auto-calculate ranges from remaining units after deleting
        property.CalculateRangesFromUnits();
        await _propertyRepository.UpdateAsync(property);
    }

    // Private helper methods

    private static bool CanUserModifyUnit(Property property, string userId, string? userRole)
    {
        var isOwner = property.IsOwnedBy(userId);
        var isAdmin = userRole == "admin" || userRole == "superadmin";

        return isOwner || isAdmin;
    }

    private static void UpdateUnitStatus(Unit unit, string status)
    {
        var newStatus = Enum.Parse<UnitStatus>(status, ignoreCase: true);

        switch (newStatus)
        {
            case UnitStatus.Available:
                unit.MakeAvailable();
                break;
            case UnitStatus.Reserved:
                unit.Reserve();
                break;
            case UnitStatus.Sold:
                unit.MarkAsSold();
                break;
            default:
                throw new ArgumentException($"Invalid status: {status}");
        }
    }

    private static UnitResponseDto MapToDto(Unit unit)
    {
        return new UnitResponseDto
        {
            Id = unit.Id,
            PropertyId = unit.PropertyId,
            UnitNumber = unit.UnitNumber,
            Floor = unit.Floor,
            Type = unit.Type,
            Bedrooms = unit.Bedrooms,
            Bathrooms = unit.Bathrooms,
            Area = unit.Area,
            Price = unit.Price,
            Status = unit.Status.ToString(),
            Images = unit.Images,
            FloorPlans = unit.FloorPlans,
            Amenities = unit.Amenities,
            Description = unit.Description,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }
}
