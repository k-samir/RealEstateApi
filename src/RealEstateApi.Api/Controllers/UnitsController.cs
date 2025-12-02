using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Exceptions;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Exceptions;
using System.Security.Claims;

namespace RealEstateApi.Api.Controllers;

/// <summary>
/// Units API Controller
/// Thin HTTP adapter - delegates to Application Service
/// </summary>
[ApiController]
[Route("api/properties/{propertyId}/units")]
[Produces("application/json")]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;
    private readonly ILogger<UnitsController> _logger;

    public UnitsController(
        IUnitService unitService,
        ILogger<UnitsController> logger)
    {
        _unitService = unitService;
        _logger = logger;
    }

    /// <summary>
    /// Get all units for a property
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UnitResponseDto>>> GetUnits(int propertyId)
    {
        try
        {
            var units = await _unitService.GetUnitsByPropertyIdAsync(propertyId);
            return Ok(units);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching units for property {PropertyId}", propertyId);
            return StatusCode(500, new { message = "An error occurred while fetching units" });
        }
    }

    /// <summary>
    /// Get unit by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UnitResponseDto>> GetUnit(int propertyId, string id)
    {
        try
        {
            var unit = await _unitService.GetUnitByIdAsync(id);

            // Verify unit belongs to the specified property
            if (unit.PropertyId != propertyId)
                return NotFound(new { message = "Unit not found for this property" });

            return Ok(unit);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unit {UnitId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the unit" });
        }
    }

    /// <summary>
    /// Create a new unit for a property (Agent/Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<UnitResponseDto>> CreateUnit(
        int propertyId,
        [FromBody] CreateUnitDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var unit = await _unitService.CreateUnitAsync(propertyId, dto, userId);
            return CreatedAtAction(nameof(GetUnit), new { propertyId, id = unit.Id }, unit);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating unit for property {PropertyId}", propertyId);
            return StatusCode(500, new { message = "An error occurred while creating the unit" });
        }
    }

    /// <summary>
    /// Update an existing unit (Agent/Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<UnitResponseDto>> UpdateUnit(
        int propertyId,
        string id,
        [FromBody] UpdateUnitDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            var unit = await _unitService.UpdateUnitAsync(id, dto, userId, userRole);

            // Verify unit belongs to the specified property
            if (unit.PropertyId != propertyId)
                return NotFound(new { message = "Unit not found for this property" });

            return Ok(unit);
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
            _logger.LogError(ex, "Error updating unit {UnitId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the unit" });
        }
    }

    /// <summary>
    /// Delete a unit (Agent/Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult> DeleteUnit(int propertyId, string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            await _unitService.DeleteUnitAsync(id, userId, userRole);
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
            _logger.LogError(ex, "Error deleting unit {UnitId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the unit" });
        }
    }
}
