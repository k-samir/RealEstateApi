using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

/// <summary>
/// UserPermissions entity - manages granular permissions for users
/// Links to Better Auth user table via UserId
/// </summary>
public class UserPermissions
{
    public string UserId { get; private set; } = string.Empty;
    public string Role { get; private set; } = "user"; // user, admin, superadmin

    // Granular Permissions (stored as JSON in database)
    public PropertyPermissions Properties { get; private set; } = new();
    public ClientPermissions Clients { get; private set; } = new();
    public TransactionPermissions Transactions { get; private set; } = new();

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private UserPermissions() { }

    /// <summary>
    /// Create user permissions with default permissions
    /// Admin/superadmin can customize these later
    /// </summary>
    public static UserPermissions Create(string userId, string role)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required");

        if (string.IsNullOrWhiteSpace(role))
            throw new DomainException("Role is required");

        var now = DateTime.UtcNow;

        var permissions = new UserPermissions
        {
            UserId = userId,
            Role = role,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Set default permissions
        permissions.ApplyDefaultPermissions();

        return permissions;
    }

    /// <summary>
    /// Apply default permissions for regular users
    /// Users can create, view, edit, and delete their own items
    /// </summary>
    public void ApplyDefaultPermissions()
    {
        Properties = PropertyPermissions.CreateDefaultPermissions();
        Clients = ClientPermissions.CreateDefaultPermissions();
        Transactions = TransactionPermissions.CreateDefaultPermissions();

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update specific permissions (for admin customization)
    /// </summary>
    public void UpdatePermissions(
        PropertyPermissions? properties = null,
        ClientPermissions? clients = null,
        TransactionPermissions? transactions = null)
    {
        if (properties != null)
            Properties = properties;

        if (clients != null)
            Clients = clients;

        if (transactions != null)
            Transactions = transactions;

        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Property permissions value object
/// </summary>
public class PropertyPermissions
{
    public bool ViewOwn { get; set; }
    public bool ViewAll { get; set; }
    public bool Create { get; set; }
    public bool EditOwn { get; set; }
    public bool EditAll { get; set; }
    public bool DeleteOwn { get; set; }
    public bool Publish { get; set; }

    /// <summary>
    /// Default permissions: users can create, view, edit, and delete their own properties
    /// </summary>
    public static PropertyPermissions CreateDefaultPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = false,
        Create = true,
        EditOwn = true,
        EditAll = false,
        DeleteOwn = true,
        Publish = false
    };
}

/// <summary>
/// Client permissions value object
/// </summary>
public class ClientPermissions
{
    public bool ViewOwn { get; set; }
    public bool ViewAll { get; set; }
    public bool Create { get; set; }
    public bool EditOwn { get; set; }
    public bool EditAll { get; set; }
    public bool DeleteOwn { get; set; }

    /// <summary>
    /// Default permissions: users can create, view, edit, and delete their own clients
    /// </summary>
    public static ClientPermissions CreateDefaultPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = false,
        Create = true,
        EditOwn = true,
        EditAll = false,
        DeleteOwn = true
    };
}

/// <summary>
/// Transaction permissions value object
/// </summary>
public class TransactionPermissions
{
    public bool ViewOwn { get; set; }
    public bool ViewAll { get; set; }
    public bool Create { get; set; }
    public bool EditOwn { get; set; }
    public bool EditAll { get; set; }
    public bool AddPayment { get; set; }

    /// <summary>
    /// Default permissions: users can create, view, edit their own transactions and add payments
    /// </summary>
    public static TransactionPermissions CreateDefaultPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = false,
        Create = true,
        EditOwn = true,
        EditAll = false,
        AddPayment = true
    };
}
