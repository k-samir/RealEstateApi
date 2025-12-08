namespace RealEstateApi.Application.Interfaces;

/// <summary>
/// Authorization service interface
/// Handles permission checks for properties, clients, and transactions
/// </summary>
public interface IAuthorizationService
{
    // Property Permissions
    Task<bool> CanViewPropertyAsync(string userId, int propertyId);
    Task<bool> CanCreatePropertyAsync(string userId);
    Task<bool> CanEditPropertyAsync(string userId, int propertyId);
    Task<bool> CanDeletePropertyAsync(string userId, int propertyId);
    Task<bool> CanPublishPropertyAsync(string userId, int propertyId);
    Task<string?> GetDeletePropertyBlockedReasonAsync(string userId, int propertyId);

    // Client Permissions
    Task<bool> CanViewClientAsync(string userId, int clientId);
    Task<bool> CanCreateClientAsync(string userId);
    Task<bool> CanEditClientAsync(string userId, int clientId);
    Task<bool> CanDeleteClientAsync(string userId, int clientId);

    // Transaction Permissions
    Task<bool> CanViewTransactionAsync(string userId, int transactionId);
    Task<bool> CanCreateTransactionAsync(string userId);
    Task<bool> CanEditTransactionAsync(string userId, int transactionId);
    Task<bool> CanDeleteTransactionAsync(string userId);
    Task<bool> CanAddPaymentAsync(string userId, int transactionId);

    // Permission Retrieval
    Task<Domain.Entities.UserPermissions?> GetUserPermissionsAsync(string userId);
    Task<bool> IsAdminAsync(string userId);
    Task<bool> IsSuperAdminAsync(string userId);
}
