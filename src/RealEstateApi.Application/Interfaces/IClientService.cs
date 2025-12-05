using RealEstateApi.Application.DTOs;

namespace RealEstateApi.Application.Interfaces;

public interface IClientService
{
    Task<(IEnumerable<ClientDto> items, int total)> GetAllAsync(string? searchQuery, int page, int pageSize);
    Task<ClientDto?> GetByIdAsync(int id);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
    Task UpdateAsync(int id, UpdateClientDto dto);
    Task DeleteAsync(int id);
}
