using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using System.Security.Claims;

namespace RealEstateApi.Api.Controllers;

/// <summary>
/// Properties API Controller
/// Manages real estate property listings
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(
        IPropertyRepository propertyRepository,
        ILogger<PropertiesController> logger)
    {
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all properties with optional filtering
    /// </summary>
    /// <param name="location">Filter by location</param>
    /// <param name="type">Filter by property type</param>
    /// <param name="status">Filter by status (published, draft, etc.)</param>
    /// <param name="search">Search query (name, description, location)</param>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetProperties(
        [FromQuery] string? location = null,
        [FromQuery] string? type = null,
        [FromQuery] string? status = "published",
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var filter = new PropertyFilter
            {
                Location = location,
                Type = type,
                Status = status,
                SearchQuery = search,
                Page = page,
                PageSize = pageSize
            };

            var properties = await _propertyRepository.GetAllAsync(filter);

            var response = properties.Select(MapToDto);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching properties");
            return StatusCode(500, new { message = "An error occurred while fetching properties" });
        }
    }

    /// <summary>
    /// Get property by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyResponseDto>> GetProperty(int id)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(id);

            if (property == null)
                return NotFound(new { message = $"Property with ID {id} not found" });

            return Ok(MapToDto(property));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching property {PropertyId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the property" });
        }
    }

    /// <summary>
    /// Get properties for the authenticated agent
    /// </summary>
    [HttpGet("my-properties")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetMyProperties()
    {
        try
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId))
                return Unauthorized(new { message = "User ID not found in token" });

            var properties = await _propertyRepository.GetByAgentIdAsync(agentId);

            var response = properties.Select(MapToDto);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching agent properties");
            return StatusCode(500, new { message = "An error occurred while fetching your properties" });
        }
    }

    /// <summary>
    /// Create a new property (agents only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<PropertyResponseDto>> CreateProperty([FromBody] CreatePropertyDto dto)
    {
        try
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(agentId))
                return Unauthorized(new { message = "User ID not found in token" });

            var property = new Property
            {
                AgentId = agentId,
                Name = dto.Name,
                Location = dto.Location,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Type = dto.Type,
                Status = "draft", // New properties start as draft
                CompletionDate = dto.CompletionDate,
                Description = dto.Description,
                LongDescription = dto.LongDescription,
                Features = dto.Features,
                Amenities = dto.Amenities,
                Specifications = dto.Specifications,
                MainImage = dto.MainImage,
                Images = dto.Images,
                BedroomsRange = dto.BedroomsRange,
                BathroomsRange = dto.BathroomsRange,
                AreaRange = dto.AreaRange,
                PriceRange = dto.PriceRange
            };

            var created = await _propertyRepository.CreateAsync(property);

            return CreatedAtAction(
                nameof(GetProperty),
                new { id = created.Id },
                MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property");
            return StatusCode(500, new { message = "An error occurred while creating the property" });
        }
    }

    /// <summary>
    /// Update property (owner or admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<PropertyResponseDto>> UpdateProperty(int id, [FromBody] UpdatePropertyDto dto)
    {
        try
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(agentId))
                return Unauthorized(new { message = "User ID not found in token" });

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                return NotFound(new { message = $"Property with ID {id} not found" });

            // Check ownership or admin role
            var isOwner = property.AgentId == agentId;
            var isAdmin = userRole == "admin" || userRole == "superadmin";

            if (!isOwner && !isAdmin)
                return Forbid();

            // Update only provided fields
            if (dto.Name != null) property.Name = dto.Name;
            if (dto.Location != null) property.Location = dto.Location;
            if (dto.Latitude.HasValue) property.Latitude = dto.Latitude;
            if (dto.Longitude.HasValue) property.Longitude = dto.Longitude;
            if (dto.Type != null) property.Type = dto.Type;
            if (dto.Status != null) property.Status = dto.Status;
            if (dto.CompletionDate != null) property.CompletionDate = dto.CompletionDate;
            if (dto.Description != null) property.Description = dto.Description;
            if (dto.LongDescription != null) property.LongDescription = dto.LongDescription;
            if (dto.Features != null) property.Features = dto.Features;
            if (dto.Amenities != null) property.Amenities = dto.Amenities;
            if (dto.Specifications != null) property.Specifications = dto.Specifications;
            if (dto.MainImage != null) property.MainImage = dto.MainImage;
            if (dto.Images != null) property.Images = dto.Images;
            if (dto.BedroomsRange != null) property.BedroomsRange = dto.BedroomsRange;
            if (dto.BathroomsRange != null) property.BathroomsRange = dto.BathroomsRange;
            if (dto.AreaRange != null) property.AreaRange = dto.AreaRange;
            if (dto.PriceRange != null) property.PriceRange = dto.PriceRange;

            var updated = await _propertyRepository.UpdateAsync(property);

            return Ok(MapToDto(updated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property {PropertyId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the property" });
        }
    }

    /// <summary>
    /// Delete property (owner or admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        try
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(agentId))
                return Unauthorized(new { message = "User ID not found in token" });

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                return NotFound(new { message = $"Property with ID {id} not found" });

            // Check ownership or admin role
            var isOwner = property.AgentId == agentId;
            var isAdmin = userRole == "admin" || userRole == "superadmin";

            if (!isOwner && !isAdmin)
                return Forbid();

            await _propertyRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property {PropertyId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the property" });
        }
    }

    /// <summary>
    /// Map Property entity to DTO
    /// </summary>
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
            Status = property.Status,
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
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList(),
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
