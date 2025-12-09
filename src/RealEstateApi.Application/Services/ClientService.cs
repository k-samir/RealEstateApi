using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _repository;

    public ClientService(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<(IEnumerable<ClientDto> items, int total)> GetAllAsync(string? searchQuery, int page, int pageSize)
    {
        var (items, total) = await _repository.GetAllAsync(searchQuery, page, pageSize);
        
        var dtos = items.Select(c => new ClientDto(
            c.Id,
            c.AgentId,
            c.FullName,
            c.Phone,
            c.Email,
            c.NationalId,
            c.Address,
            c.Notes,
            c.CreatedAt
        ));
        
        return (dtos, total);
    }

    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        var c = await _repository.GetByIdAsync(id);
        if (c == null) return null;

        return new ClientDto(
            c.Id,
            c.AgentId,
            c.FullName,
            c.Phone,
            c.Email,
            c.NationalId,
            c.Address,
            c.Notes,
            c.CreatedAt
        );
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var client = Client.Create(dto.AgentId, dto.FullName, dto.Phone, dto.Email, dto.NationalId, dto.Address ?? string.Empty, dto.Notes);
        await _repository.CreateAsync(client);

        return new ClientDto(
            client.Id,
            client.AgentId,
            client.FullName,
            client.Phone,
            client.Email,
            client.NationalId,
            client.Address,
            client.Notes,
            client.CreatedAt
        );
    }

    public async Task UpdateAsync(int id, UpdateClientDto dto)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client == null) throw new DomainException($"Client with ID {id} not found");

        client.Update(dto.FullName, dto.Phone, dto.Email, dto.NationalId, dto.Address, dto.Notes);
        await _repository.UpdateAsync(client);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
