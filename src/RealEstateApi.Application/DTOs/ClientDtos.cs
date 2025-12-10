namespace RealEstateApi.Application.DTOs;

public record ClientDto(
    int Id,
    string AgentId,
    string FullName,
    string Phone,
    string Email,
    string NationalId,
    string Address,
    string? Notes,
    DateTime CreatedAt
);

public record CreateClientDto(
    string AgentId,
    string FullName,
    string Phone,
    string Email,
    string NationalId,
    string Address,
    string? Notes
);

public record UpdateClientDto(
    string FullName,
    string Phone,
    string Email,
    string NationalId,
    string Address,
    string? Notes
);
