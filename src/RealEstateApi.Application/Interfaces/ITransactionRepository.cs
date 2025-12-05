using RealEstateApi.Domain.Entities;

namespace RealEstateApi.Application.Interfaces;

public interface ITransactionRepository
{
    Task<(List<Transaction> items, int total)> GetAllAsync(int? clientId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<bool> HasActiveTransactionForUnitAsync(string unitId, CancellationToken cancellationToken = default);
    Task<List<Transaction>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
}
