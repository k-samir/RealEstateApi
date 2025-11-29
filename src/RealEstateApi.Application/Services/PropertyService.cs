using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Exceptions;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;
using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Application.Services;

/// <summary>
/// Property service - implements property management use cases
/// Orchestrates domain logic and repository operations
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyResponseDto> GetPropertyByIdAsync(int id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
            throw new NotFoundException($"Property with ID {id} not found");

        return MapToDto(property);
    }

    public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync(PropertyFilter filter)
    {
        var properties = await _propertyRepository.GetAllAsync(filter);
        return properties.Select(MapToDto);
    }

    public async Task<IEnumerable<PropertyResponseDto>> GetAgentPropertiesAsync(string agentId)
    {
        var properties = await _propertyRepository.GetByAgentIdAsync(agentId);
        return properties.Select(MapToDto);
    }

    public async Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto dto, string agentId)
    {
        // Use domain factory method - enforces business rules
        var property = Property.Create(
            agentId,
            dto.Name,
            dto.Location,
            dto.Type,
            dto.Description
        );

        // Update additional details through domain methods
        if (dto.LongDescription != null || dto.Latitude.HasValue || dto.Longitude.HasValue || dto.CompletionDate != null)
        {
            property.UpdateDetails(
                dto.Name,
                dto.Location,
                dto.Type,
                dto.Description,
                dto.LongDescription,
                dto.Latitude,
                dto.Longitude,
                dto.CompletionDate
            );
        }

        // Update features and amenities if provided
        if (dto.Features != null || dto.Amenities != null || dto.Specifications != null)
        {
            property.UpdateFeaturesAndAmenities(
                dto.Features,
                dto.Amenities,
                dto.Specifications
            );
        }

        // Update images if provided
        if (dto.MainImage != null || dto.Images != null)
        {
            property.UpdateImages(dto.MainImage, dto.Images);
        }

        // Update ranges if provided
        if (dto.BedroomsRange != null || dto.BathroomsRange != null || dto.AreaRange != null || dto.PriceRange != null)
        {
            property.UpdateRanges(
                dto.BedroomsRange,
                dto.BathroomsRange,
                dto.AreaRange,
                dto.PriceRange
            );
        }

        var created = await _propertyRepository.CreateAsync(property);

        return MapToDto(created);
    }

    public async Task<PropertyResponseDto> UpdatePropertyAsync(int id, UpdatePropertyDto dto, string userId, string? userRole)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
            throw new NotFoundException($"Property with ID {id} not found");

        // Check authorization - use domain method
        if (!CanUserModifyProperty(property, userId, userRole))
            throw new ForbiddenException("You don't have permission to update this property");

        // Check if property can be edited
        if (!property.IsEditable())
            throw new DomainException("This property cannot be edited");

        // Update basic details if provided
        if (dto.Name != null || dto.Location != null || dto.Type != null || dto.Description != null)
        {
            property.UpdateDetails(
                dto.Name ?? property.Name,
                dto.Location ?? property.Location,
                dto.Type ?? property.Type,
                dto.Description ?? property.Description,
                dto.LongDescription,
                dto.Latitude,
                dto.Longitude,
                dto.CompletionDate
            );
        }

        // Update features and amenities if provided
        if (dto.Features != null || dto.Amenities != null || dto.Specifications != null)
        {
            property.UpdateFeaturesAndAmenities(
                dto.Features,
                dto.Amenities,
                dto.Specifications
            );
        }

        // Update images if provided
        if (dto.MainImage != null || dto.Images != null)
        {
            property.UpdateImages(dto.MainImage, dto.Images);
        }

        // Update ranges if provided
        if (dto.BedroomsRange != null || dto.BathroomsRange != null || dto.AreaRange != null || dto.PriceRange != null)
        {
            property.UpdateRanges(
                dto.BedroomsRange,
                dto.BathroomsRange,
                dto.AreaRange,
                dto.PriceRange
            );
        }

        // Handle status changes through domain methods
        if (dto.Status != null)
        {
            UpdatePropertyStatus(property, dto.Status);
        }

        var updated = await _propertyRepository.UpdateAsync(property);

        return MapToDto(updated);
    }

    public async Task DeletePropertyAsync(int id, string userId, string? userRole)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
            throw new NotFoundException($"Property with ID {id} not found");

        // Check authorization - use domain method
        if (!CanUserModifyProperty(property, userId, userRole))
            throw new ForbiddenException("You don't have permission to delete this property");

        await _propertyRepository.DeleteAsync(id);
    }

    public async Task<PropertyResponseDto> PublishPropertyAsync(int id, string userId)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
            throw new NotFoundException($"Property with ID {id} not found");

        // Check ownership
        if (!property.IsOwnedBy(userId))
            throw new ForbiddenException("You don't have permission to publish this property");

        // Use domain method - enforces business rules
        property.Publish();

        var updated = await _propertyRepository.UpdateAsync(property);

        return MapToDto(updated);
    }

    // Private helper methods

    private static bool CanUserModifyProperty(Property property, string userId, string? userRole)
    {
        var isOwner = property.IsOwnedBy(userId);
        var isAdmin = userRole == "admin" || userRole == "superadmin";

        return isOwner || isAdmin;
    }

    private static void UpdatePropertyStatus(Property property, string status)
    {
        var newStatus = Enum.Parse<PropertyStatus>(status, ignoreCase: true);

        switch (newStatus)
        {
            case PropertyStatus.Published:
                property.Publish();
                break;
            case PropertyStatus.Reserved:
                property.Reserve();
                break;
            case PropertyStatus.Sold:
                property.MarkAsSold();
                break;
            case PropertyStatus.Draft:
                property.RevertToDraft();
                break;
            default:
                throw new ArgumentException($"Invalid status: {status}");
        }
    }

    private static PropertyResponseDto MapToDto(Property property)
    {
        return new PropertyResponseDto
        {
            Id = property.Id,
            AgentId = property.AgentId,
            Name = property.Name,
            Location = property.Location,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            Type = property.Type,
            Status = property.Status.ToString(),
            CompletionDate = property.CompletionDate,
            Description = property.Description,
            LongDescription = property.LongDescription,
            Features = property.Features,
            Amenities = property.Amenities,
            Specifications = property.Specifications,
            MainImage = property.MainImage,
            Images = property.Images,
            BedroomsRange = property.BedroomsRange,
            BathroomsRange = property.BathroomsRange,
            AreaRange = property.AreaRange,
            PriceRange = property.PriceRange,
            Units = property.Units.Select(u => new UnitResponseDto
            {
                Id = u.Id,
                PropertyId = u.PropertyId,
                UnitNumber = u.UnitNumber,
                Floor = u.Floor,
                Type = u.Type,
                Bedrooms = u.Bedrooms,
                Bathrooms = u.Bathrooms,
                Area = u.Area,
                Price = u.Price,
                Status = u.Status.ToString(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList(),
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
