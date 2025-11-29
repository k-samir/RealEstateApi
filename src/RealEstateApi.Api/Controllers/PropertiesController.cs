using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Exceptions;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Exceptions;
using System.Security.Claims;

namespace RealEstateApi.Api.Controllers;

/// <summary>
/// Properties API Controller
/// Thin HTTP adapter - delegates to Application Service
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(
        IPropertyService propertyService,
        ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService;
        _logger = logger;
    }

    /// <summary>
    /// Get all properties with optional filtering
    /// </summary>
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

            var properties = await _propertyService.GetAllPropertiesAsync(filter);
            return Ok(properties);
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
            var property = await _propertyService.GetPropertyByIdAsync(id);
            return Ok(property);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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

            var properties = await _propertyService.GetAgentPropertiesAsync(agentId);
            return Ok(properties);
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

            var property = await _propertyService.CreatePropertyAsync(dto, agentId);

            return CreatedAtAction(
                nameof(GetProperty),
                new { id = property.Id },
                property);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var property = await _propertyService.UpdatePropertyAsync(id, dto, userId, userRole);
            return Ok(property);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            await _propertyService.DeletePropertyAsync(id, userId, userRole);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property {PropertyId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the property" });
        }
    }

    /// <summary>
    /// Publish a property (owner only)
    /// </summary>
    [HttpPost("{id}/publish")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<PropertyResponseDto>> PublishProperty(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var property = await _propertyService.PublishPropertyAsync(id, userId);
            return Ok(property);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing property {PropertyId}", id);
            return StatusCode(500, new { message = "An error occurred while publishing the property" });
        }
    }
}
