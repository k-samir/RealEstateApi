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

    public async Task<PagedPropertiesResponse> GetAllPropertiesAsync(PropertyFilter filter)
    {
        var (items, total) = await _propertyRepository.GetAllAsync(filter);
        var projects = items.Select(MapToDto).ToList();
        
        var totalPages = (int)Math.Ceiling(total / (double)filter.PageSize);
        
        return new PagedPropertiesResponse
        {
            Projects = projects,
            Total = total,
            Page = filter.Page,
            TotalPages = totalPages
        };
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

        // Update basic info
        property.UpdateBasicInfo(
            dto.Name,
            dto.Type,
            dto.Description,
            dto.Developer,
            dto.Category,
            dto.IsFeatured
        );

        // CHANGED: Handle PropertyMode
        if (!string.IsNullOrEmpty(dto.PropertyMode))
        {
            var mode = Enum.Parse<PropertyMode>(dto.PropertyMode, ignoreCase: true);
            property.SetMode(mode);
        }

        // Update location
        property.UpdateLocation(
            dto.Location,
            dto.StreetAddress,
            dto.Area,
            dto.City,
            dto.State,
            dto.Country,
            dto.PostalCode,
            dto.Latitude,
            dto.Longitude
        );

        // Update project details
        property.UpdateProjectDetails(
            dto.PriceRange,
            dto.CompletionDate,
            dto.TotalUnits,
            dto.TotalFloors,
            dto.TotalLandArea,
            dto.LandAreaUnit,
            dto.UnitTypesAvailable
        );

        // REMOVED: UpdateUnitSpecifications - ranges now calculated from units
        // Update descriptions
        property.UpdateDescriptions(
            dto.Description,
            dto.LongDescription,
            dto.KeyHighlights
        );

        // Update amenities and specifications
        property.UpdateAmenitiesAndSpecifications(
            dto.Amenities,
            dto.Specifications,
            dto.NearbyPlaces
        );

        // Update media
        property.UpdateMedia(
            dto.MainImage,
            dto.Images,
            dto.FloorPlans,
            dto.VideoTourUrl
        );

        // Set publish status if provided
        if (dto.IsPublished)
        {
            property.SetPublishStatus(dto.IsPublished);
        }

        // Save property first to get the PropertyId for units
        var created = await _propertyRepository.CreateAsync(property);

        // ADDED: Handle Units array
        if (dto.Units != null && dto.Units.Any())
        {
            // Create units from DTO
            foreach (var unitDto in dto.Units)
            {
                var unit = Unit.Create(
                    created.Id,
                    unitDto.UnitNumber,
                    unitDto.Type,
                    unitDto.Bedrooms,
                    unitDto.Bathrooms,
                    unitDto.Area,
                    unitDto.Price,
                    unitDto.Floor,
                    unitDto.Images,
                    unitDto.FloorPlans,
                    unitDto.Amenities,
                    unitDto.Description
                );
                created.AddUnit(unit);
            }
        }
        else if (created.Mode == PropertyMode.Standalone)
        {
            // ADDED: If Standalone mode and no units provided, create a default unit
            var defaultUnit = Unit.Create(
                created.Id,
                "Unit 1",
                "Unit",
                0, // bedrooms
                0, // bathrooms
                0, // area
                0  // price
            );
            created.AddUnit(defaultUnit);
        }

        // ADDED: Calculate ranges from units after adding them
        if (created.Units.Any())
        {
            created.CalculateRangesFromUnits();
            await _propertyRepository.UpdateAsync(created);
        }

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

        // Handle PropertyMode changes
        if (!string.IsNullOrEmpty(dto.PropertyMode))
        {
            var mode = Enum.Parse<PropertyMode>(dto.PropertyMode, ignoreCase: true);
            property.SetMode(mode);
        }

        // REMOVED: Standalone unit details handling - now uses units array

        // Update basic info if provided
        if (dto.Name != null || dto.Type != null || dto.Description != null || dto.Developer != null || dto.Category != null || dto.IsFeatured.HasValue)
        {
            property.UpdateBasicInfo(
                dto.Name ?? property.Name,
                dto.Type ?? property.Type,
                dto.Description ?? property.Description,
                dto.Developer,
                dto.Category,
                dto.IsFeatured
            );
        }

        // Update location if provided
        if (dto.Location != null || dto.StreetAddress != null || dto.Area != null || dto.City != null ||
            dto.State != null || dto.Country != null || dto.PostalCode != null ||
            dto.Latitude.HasValue || dto.Longitude.HasValue)
        {
            property.UpdateLocation(
                dto.Location ?? property.Location,
                dto.StreetAddress,
                dto.Area,
                dto.City,
                dto.State,
                dto.Country,
                dto.PostalCode,
                dto.Latitude,
                dto.Longitude
            );
        }

        // Update project details if provided
        if (dto.PriceRange != null || dto.CompletionDate != null || dto.TotalUnits.HasValue ||
            dto.TotalFloors.HasValue || dto.TotalLandArea.HasValue || dto.LandAreaUnit != null ||
            dto.UnitTypesAvailable != null)
        {
            property.UpdateProjectDetails(
                dto.PriceRange,
                dto.CompletionDate,
                dto.TotalUnits,
                dto.TotalFloors,
                dto.TotalLandArea,
                dto.LandAreaUnit,
                dto.UnitTypesAvailable
            );
        }

        // REMOVED: UpdateUnitSpecifications - ranges now calculated from units automatically

        // Update descriptions if provided
        if (dto.Description != null || dto.LongDescription != null || dto.KeyHighlights != null)
        {
            property.UpdateDescriptions(
                dto.Description,
                dto.LongDescription,
                dto.KeyHighlights
            );
        }

        // Update amenities and specifications if provided
        if (dto.Amenities != null || dto.Specifications != null || dto.NearbyPlaces != null)
        {
            property.UpdateAmenitiesAndSpecifications(
                dto.Amenities,
                dto.Specifications,
                dto.NearbyPlaces
            );
        }

        // Update media if provided
        if (dto.MainImage != null || dto.Images != null || dto.FloorPlans != null || dto.VideoTourUrl != null)
        {
            property.UpdateMedia(
                dto.MainImage,
                dto.Images,
                dto.FloorPlans,
                dto.VideoTourUrl
            );
        }

        // ADDED: Handle Units array update
        if (dto.Units != null)
        {
            // Clear existing units (RemoveUnit handles business rules)
            var existingUnits = property.Units.ToList();
            foreach (var existingUnit in existingUnits)
            {
                property.RemoveUnit(existingUnit);
            }

            // Add new units from DTO
            foreach (var unitDto in dto.Units)
            {
                var unit = Unit.Create(
                    property.Id,
                    unitDto.UnitNumber,
                    unitDto.Type,
                    unitDto.Bedrooms,
                    unitDto.Bathrooms,
                    unitDto.Area,
                    unitDto.Price,
                    unitDto.Floor,
                    unitDto.Images,
                    unitDto.FloorPlans,
                    unitDto.Amenities,
                    unitDto.Description
                );
                property.AddUnit(unit);
            }

            // ADDED: Calculate ranges from units after updating them
            property.CalculateRangesFromUnits();
        }

        // Handle status changes through domain methods
        // Only update if status has actually changed
        if (dto.Status != null)
        {
            // Normalize the incoming status for comparison
            var normalizedIncomingStatus = dto.Status switch
            {
                "Published" => "Active",
                "Reserved" => "Active",
                _ => dto.Status
            };

            // Only update if different from current status
            if (property.Status.ToString() != normalizedIncomingStatus)
            {
                UpdatePropertyStatus(property, dto.Status);
            }
        }

        // Handle publish status changes
        if (dto.IsPublished.HasValue)
        {
            property.SetPublishStatus(dto.IsPublished.Value);
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
        // Handle backward compatibility with old status values
        var normalizedStatus = status switch
        {
            "Published" => "Active",  // Map old Published to Active
            "Reserved" => "Active",   // Map old Reserved to Active
            _ => status
        };

        var newStatus = Enum.Parse<PropertyStatus>(normalizedStatus, ignoreCase: true);

        switch (newStatus)
        {
            case PropertyStatus.Draft:
            case PropertyStatus.Active:
            case PropertyStatus.UnderConstruction:
            case PropertyStatus.Completed:
                property.UpdateStatus(newStatus);
                break;
            case PropertyStatus.Sold:
                property.MarkAsSold();
                break;
            case PropertyStatus.Rented:
                property.MarkAsRented();
                break;
            case PropertyStatus.Archived:
                property.Archive();
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

            // Basic Information
            Name = property.Name,
            Developer = property.Developer,
            Category = property.Category,
            Type = property.Type,
            Status = property.Status.ToString(),
            IsPublished = property.IsPublished,
            IsFeatured = property.IsFeatured,

            // Property Mode
            PropertyMode = property.Mode.ToString(),

            // REMOVED: Standalone Unit Fields (Bedrooms, Bathrooms, UnitArea, UnitPrice, AreaUnit)
            // These are now only in the Units array

            // Location
            Location = property.Location,
            StreetAddress = property.StreetAddress,
            Area = property.Area,
            City = property.City,
            State = property.State,
            Country = property.Country,
            PostalCode = property.PostalCode,
            Latitude = property.Latitude,
            Longitude = property.Longitude,

            // Pricing
            PriceRange = property.PriceRange,

            // Project Details
            CompletionDate = property.CompletionDate,
            TotalUnits = property.TotalUnits,
            TotalFloors = property.TotalFloors,
            TotalLandArea = property.TotalLandArea,
            LandAreaUnit = property.LandAreaUnit,
            UnitTypesAvailable = property.UnitTypesAvailable,

            // Unit Specifications (calculated from units)
            BedroomsRange = property.BedroomsRange,
            BathroomsRange = property.BathroomsRange,
            AreaRange = property.AreaRange,
            FurnishingStatus = property.FurnishingStatus,

            // Descriptions
            Description = property.Description,
            LongDescription = property.LongDescription,
            KeyHighlights = property.KeyHighlights,

            // Amenities & Specifications
            Amenities = property.Amenities,
            Specifications = property.Specifications,
            NearbyPlaces = property.NearbyPlaces,

            // Media
            MainImage = property.MainImage,
            Images = property.Images,
            FloorPlans = property.FloorPlans,
            VideoTourUrl = property.VideoTourUrl,

            // Units
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
                Images = u.Images,
                FloorPlans = u.FloorPlans,
                Amenities = u.Amenities,
                Description = u.Description,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList(),

            // Metadata
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
