using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.Services;

/// <summary>
/// Authorization service - implements permission checks
/// Validates user permissions for properties, clients, and transactions
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IUserPermissionsRepository _permissionsRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AuthorizationService(
        IUserPermissionsRepository permissionsRepository,
        IPropertyRepository propertyRepository,
        IClientRepository clientRepository,
        ITransactionRepository transactionRepository)
    {
        _permissionsRepository = permissionsRepository;
        _propertyRepository = propertyRepository;
        _clientRepository = clientRepository;
        _transactionRepository = transactionRepository;
    }

    // ============================================================================
    // PROPERTY PERMISSIONS
    // ============================================================================

    public async Task<bool> CanViewPropertyAsync(string userId, int propertyId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can view all
        if (await IsAdminAsync(userId)) return true;

        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null) return false;

        // Check granular permissions
        if (permissions.Properties.ViewAll) return true;
        if (permissions.Properties.ViewOwn && property.AgentId == userId) return true;

        return false;
    }

    public async Task<bool> CanCreatePropertyAsync(string userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can always create
        if (await IsAdminAsync(userId)) return true;

        return permissions.Properties.Create;
    }

    public async Task<bool> CanEditPropertyAsync(string userId, int propertyId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can edit all
        if (await IsAdminAsync(userId)) return true;

        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null) return false;

        // Check granular permissions
        if (permissions.Properties.EditAll) return true;
        if (permissions.Properties.EditOwn && property.AgentId == userId) return true;

        return false;
    }

    public async Task<bool> CanDeletePropertyAsync(string userId, int propertyId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null) return false;

        // Check if property has transactions
        var hasTransactions = await HasPropertyTransactionsAsync(propertyId);

        // Only superadmin can force delete properties with transactions
        if (hasTransactions && !await IsSuperAdminAsync(userId))
            return false;

        // Admins can delete (unless has transactions)
        if (await IsAdminAsync(userId)) return true;

        // Agents can only delete own properties
        if (permissions.Properties.DeleteOwn && property.AgentId == userId && !hasTransactions)
            return true;

        return false;
    }

    public async Task<bool> CanPublishPropertyAsync(string userId, int propertyId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can publish any property
        if (await IsAdminAsync(userId)) return true;

        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null) return false;

        // Must own the property and have publish permission
        return permissions.Properties.Publish && property.AgentId == userId;
    }

    public async Task<string?> GetDeletePropertyBlockedReasonAsync(string userId, int propertyId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return "User permissions not found";

        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null) return "Property not found";

        var hasTransactions = await HasPropertyTransactionsAsync(propertyId);

        if (hasTransactions && !await IsSuperAdminAsync(userId))
            return "Property has transactions and can only be force-deleted by superadmin";

        if (property.AgentId != userId && !await IsAdminAsync(userId))
            return "You can only delete your own properties";

        if (!permissions.Properties.DeleteOwn && !await IsAdminAsync(userId))
            return "You don't have permission to delete properties";

        return null;
    }

    // ============================================================================
    // CLIENT PERMISSIONS
    // ============================================================================

    public async Task<bool> CanViewClientAsync(string userId, int clientId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can view all
        if (await IsAdminAsync(userId)) return true;

        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null) return false;

        // Check granular permissions
        if (permissions.Clients.ViewAll) return true;
        if (permissions.Clients.ViewOwn && client.AgentId == userId) return true;

        return false;
    }

    public async Task<bool> CanCreateClientAsync(string userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can always create
        if (await IsAdminAsync(userId)) return true;

        return permissions.Clients.Create;
    }

    public async Task<bool> CanEditClientAsync(string userId, int clientId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can edit all
        if (await IsAdminAsync(userId)) return true;

        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null) return false;

        // Check granular permissions
        if (permissions.Clients.EditAll) return true;
        if (permissions.Clients.EditOwn && client.AgentId == userId) return true;

        return false;
    }

    public async Task<bool> CanDeleteClientAsync(string userId, int clientId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can delete
        if (await IsAdminAsync(userId)) return true;

        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null) return false;

        // Can only delete own clients
        return permissions.Clients.DeleteOwn && client.AgentId == userId;
    }

    // ============================================================================
    // TRANSACTION PERMISSIONS
    // ============================================================================

    public async Task<bool> CanViewTransactionAsync(string userId, int transactionId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can view all
        if (await IsAdminAsync(userId)) return true;

        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) return false;

        // Check granular permissions
        if (permissions.Transactions.ViewAll) return true;
        if (permissions.Transactions.ViewOwn && transaction.AgentId == userId) return true;

        return false;
    }

    public async Task<bool> CanCreateTransactionAsync(string userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can always create
        if (await IsAdminAsync(userId)) return true;

        return permissions.Transactions.Create;
    }

    public async Task<bool> CanEditTransactionAsync(string userId, int transactionId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can edit all
        if (await IsAdminAsync(userId)) return true;

        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) return false;

        // Check granular permissions
        if (permissions.Transactions.EditAll) return true;
        if (permissions.Transactions.EditOwn && transaction.AgentId == userId) return true;

        return false;
    }

    public async Task<bool> CanDeleteTransactionAsync(string userId)
    {
        // Only superadmin can delete transactions (audit trail requirement)
        return await IsSuperAdminAsync(userId);
    }

    public async Task<bool> CanAddPaymentAsync(string userId, int transactionId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        // Admins can add payments to any transaction
        if (await IsAdminAsync(userId)) return true;

        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) return false;

        // Must have addPayment permission and own the transaction
        return permissions.Transactions.AddPayment && transaction.AgentId == userId;
    }

    // ============================================================================
    // PERMISSION RETRIEVAL & HELPERS
    // ============================================================================

    public async Task<UserPermissions?> GetUserPermissionsAsync(string userId)
    {
        var permissions = await _permissionsRepository.GetByUserIdAsync(userId);

        // If no permissions exist, create default permissions for the user
        if (permissions == null)
        {
            // Default to junior agent
            permissions = UserPermissions.Create(userId, "user", "junior");
            await _permissionsRepository.CreateAsync(permissions);
        }

        return permissions;
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        return permissions.Role == "admin" || permissions.Role == "superadmin";
    }

    public async Task<bool> IsSuperAdminAsync(string userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        if (permissions == null) return false;

        return permissions.Role == "superadmin";
    }

    // ============================================================================
    // PRIVATE HELPERS
    // ============================================================================

    private async Task<bool> HasPropertyTransactionsAsync(int propertyId)
    {
        // Check if any unit of this property has transactions
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null) return false;

        foreach (var unit in property.Units)
        {
            var hasTransactions = await _transactionRepository.HasTransactionsForUnitAsync(unit.Id);
            if (hasTransactions) return true;
        }

        return false;
    }
}
