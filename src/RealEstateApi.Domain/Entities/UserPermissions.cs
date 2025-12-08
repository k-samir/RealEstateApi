using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

/// <summary>
/// UserPermissions entity - manages granular permissions for agents
/// Links to Better Auth user table via UserId
/// </summary>
public class UserPermissions
{
    public string UserId { get; private set; } = string.Empty;
    public string Role { get; private set; } = "user"; // user, admin, superadmin
    public string? AgentTier { get; private set; } // junior, senior, teamLead

    // Granular Permissions (stored as JSON in database)
    public PropertyPermissions Properties { get; private set; } = new();
    public ClientPermissions Clients { get; private set; } = new();
    public TransactionPermissions Transactions { get; private set; } = new();

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private UserPermissions() { }

    /// <summary>
    /// Create user permissions with a preset based on agent tier
    /// </summary>
    public static UserPermissions Create(string userId, string role, string? agentTier = null)
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
            AgentTier = agentTier,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Set default permissions based on role and tier
        permissions.ApplyPreset(role, agentTier);

        return permissions;
    }

    /// <summary>
    /// Apply permission preset based on role and agent tier
    /// </summary>
    public void ApplyPreset(string role, string? agentTier = null)
    {
        if (role == "admin" || role == "superadmin")
        {
            // Admins get full permissions
            Properties = PropertyPermissions.CreateAdminPermissions();
            Clients = ClientPermissions.CreateAdminPermissions();
            Transactions = TransactionPermissions.CreateAdminPermissions();
        }
        else
        {
            // Agents get permissions based on tier
            switch (agentTier?.ToLower())
            {
                case "senior":
                    Properties = PropertyPermissions.CreateSeniorAgentPermissions();
                    Clients = ClientPermissions.CreateSeniorAgentPermissions();
                    Transactions = TransactionPermissions.CreateSeniorAgentPermissions();
                    break;

                case "teamlead":
                    Properties = PropertyPermissions.CreateTeamLeadPermissions();
                    Clients = ClientPermissions.CreateTeamLeadPermissions();
                    Transactions = TransactionPermissions.CreateTeamLeadPermissions();
                    break;

                case "junior":
                default:
                    Properties = PropertyPermissions.CreateJuniorAgentPermissions();
                    Clients = ClientPermissions.CreateJuniorAgentPermissions();
                    Transactions = TransactionPermissions.CreateJuniorAgentPermissions();
                    break;
            }
        }

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

    public static PropertyPermissions CreateJuniorAgentPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = false,
        Create = true,
        EditOwn = true,
        EditAll = false,
        DeleteOwn = false,
        Publish = false
    };

    public static PropertyPermissions CreateSeniorAgentPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = false,
        DeleteOwn = true,
        Publish = false
    };

    public static PropertyPermissions CreateTeamLeadPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = true,
        DeleteOwn = true,
        Publish = true
    };

    public static PropertyPermissions CreateAdminPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = true,
        DeleteOwn = true,
        Publish = true
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

    public static ClientPermissions CreateJuniorAgentPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = false,
        Create = true,
        EditOwn = true,
        EditAll = false,
        DeleteOwn = false
    };

    public static ClientPermissions CreateSeniorAgentPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = false,
        DeleteOwn = true
    };

    public static ClientPermissions CreateTeamLeadPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = true,
        DeleteOwn = true
    };

    public static ClientPermissions CreateAdminPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = true,
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

    public static TransactionPermissions CreateJuniorAgentPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = false,
        Create = true,
        EditOwn = true,
        EditAll = false,
        AddPayment = true
    };

    public static TransactionPermissions CreateSeniorAgentPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = false,
        AddPayment = true
    };

    public static TransactionPermissions CreateTeamLeadPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = true,
        AddPayment = true
    };

    public static TransactionPermissions CreateAdminPermissions() => new()
    {
        ViewOwn = true,
        ViewAll = true,
        Create = true,
        EditOwn = true,
        EditAll = true,
        AddPayment = true
    };
}
