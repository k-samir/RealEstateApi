using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

public class Client
{
    public int Id { get; private set; }
    public string AgentId { get; private set; } = string.Empty; // Agent who owns this client
    public string FullName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NationalId { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Client() { }

    public static Client Create(string agentId, string fullName, string phone, string email, string nationalId, string address, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(agentId)) throw new DomainException("Agent ID is required");
        if (string.IsNullOrWhiteSpace(fullName)) throw new DomainException("Full Name is required");
        if (string.IsNullOrWhiteSpace(phone)) throw new DomainException("Phone is required");

        var now = DateTime.UtcNow;
        return new Client
        {
            AgentId = agentId,
            FullName = fullName,
            Phone = phone,
            Email = email,
            NationalId = nationalId,
            Address = address,
            Notes = notes,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(string fullName, string phone, string email, string nationalId, string address, string? notes)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new DomainException("Full Name is required");
        
        FullName = fullName;
        Phone = phone;
        Email = email;
        NationalId = nationalId;
        Address = address;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
