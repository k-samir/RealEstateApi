using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Interfaces;

namespace RealEstateApi.Api.Controllers;

/// <summary>
/// User permissions management endpoints
/// Allows admins to get, update, and reset user permissions
/// </summary>
[ApiController]
[Route("api/v1/users/{userId}/permissions")]
public class UserPermissionsController : ControllerBase
{
    private readonly IUserPermissionsRepository _permissionsRepository;

    public UserPermissionsController(IUserPermissionsRepository permissionsRepository)
    {
        _permissionsRepository = permissionsRepository;
    }

    /// <summary>
    /// Get user's custom permissions
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User permissions or 404 if not found</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserPermissions(string userId)
    {
        // TODO: Add admin authorization check

        var permissions = await _permissionsRepository.GetByUserIdAsync(userId);

        if (permissions == null)
        {
            return NotFound(new { message = "User permissions not found" });
        }

        var response = new
        {
            properties = new
            {
                viewOwn = permissions.Properties.ViewOwn,
                viewAll = permissions.Properties.ViewAll,
                create = permissions.Properties.Create,
                editOwn = permissions.Properties.EditOwn,
                editAll = permissions.Properties.EditAll,
                deleteOwn = permissions.Properties.DeleteOwn,
                publish = permissions.Properties.Publish
            },
            clients = new
            {
                viewOwn = permissions.Clients.ViewOwn,
                viewAll = permissions.Clients.ViewAll,
                create = permissions.Clients.Create,
                editOwn = permissions.Clients.EditOwn,
                editAll = permissions.Clients.EditAll,
                deleteOwn = permissions.Clients.DeleteOwn
            },
            transactions = new
            {
                viewOwn = permissions.Transactions.ViewOwn,
                viewAll = permissions.Transactions.ViewAll,
                create = permissions.Transactions.Create,
                editOwn = permissions.Transactions.EditOwn,
                editAll = permissions.Transactions.EditAll,
                addPayment = permissions.Transactions.AddPayment
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Update user's custom permissions
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Permission updates</param>
    /// <returns>Success response</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateUserPermissions(string userId, [FromBody] UpdatePermissionsRequest request)
    {
        // TODO: Add admin authorization check

        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { message = "User ID is required" });
        }

        // Get existing permissions or create new
        var permissions = await _permissionsRepository.GetByUserIdAsync(userId);

        if (permissions == null)
        {
            // Create new permissions with provided values
            permissions = Domain.Entities.UserPermissions.Create(userId, "user");
        }

        // Update permissions
        var propertyPerms = new Domain.Entities.PropertyPermissions
        {
            ViewOwn = request.Properties.ViewOwn,
            ViewAll = request.Properties.ViewAll,
            Create = request.Properties.Create,
            EditOwn = request.Properties.EditOwn,
            EditAll = request.Properties.EditAll,
            DeleteOwn = request.Properties.DeleteOwn,
            Publish = request.Properties.Publish
        };

        var clientPerms = new Domain.Entities.ClientPermissions
        {
            ViewOwn = request.Clients.ViewOwn,
            ViewAll = request.Clients.ViewAll,
            Create = request.Clients.Create,
            EditOwn = request.Clients.EditOwn,
            EditAll = request.Clients.EditAll,
            DeleteOwn = request.Clients.DeleteOwn
        };

        var transactionPerms = new Domain.Entities.TransactionPermissions
        {
            ViewOwn = request.Transactions.ViewOwn,
            ViewAll = request.Transactions.ViewAll,
            Create = request.Transactions.Create,
            EditOwn = request.Transactions.EditOwn,
            EditAll = request.Transactions.EditAll,
            AddPayment = request.Transactions.AddPayment
        };

        permissions.UpdatePermissions(propertyPerms, clientPerms, transactionPerms);

        // Save to database
        if (await _permissionsRepository.ExistsAsync(userId))
        {
            await _permissionsRepository.UpdateAsync(permissions);
        }
        else
        {
            await _permissionsRepository.CreateAsync(permissions);
        }

        return Ok(new { message = "Permissions updated successfully" });
    }

    /// <summary>
    /// Reset user permissions to defaults
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success response</returns>
    [HttpDelete]
    public async Task<IActionResult> ResetUserPermissions(string userId)
    {
        // TODO: Add admin authorization check

        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { message = "User ID is required" });
        }

        await _permissionsRepository.DeleteAsync(userId);

        return Ok(new { message = "Permissions reset to defaults" });
    }
}

/// <summary>
/// Request model for updating user permissions
/// </summary>
public record UpdatePermissionsRequest(
    PropertyPermissionsDto Properties,
    ClientPermissionsDto Clients,
    TransactionPermissionsDto Transactions
);

public record PropertyPermissionsDto(
    bool ViewOwn,
    bool ViewAll,
    bool Create,
    bool EditOwn,
    bool EditAll,
    bool DeleteOwn,
    bool Publish
);

public record ClientPermissionsDto(
    bool ViewOwn,
    bool ViewAll,
    bool Create,
    bool EditOwn,
    bool EditAll,
    bool DeleteOwn
);

public record TransactionPermissionsDto(
    bool ViewOwn,
    bool ViewAll,
    bool Create,
    bool EditOwn,
    bool EditAll,
    bool AddPayment
);
