using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.Interfaces;

public interface IClientRepository
{
    Task<(List<Client> items, int total)> GetAllAsync(string? searchQuery, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default);
    Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
