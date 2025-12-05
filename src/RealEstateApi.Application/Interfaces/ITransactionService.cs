using RealEstateApi.Application.DTOs;

namespace RealEstateApi.Application.Interfaces;

public interface ITransactionService
{
    Task<(IEnumerable<TransactionDto> items, int total)> GetAllAsync(int? clientId, int page, int pageSize);
    Task<TransactionDto?> GetByIdAsync(int id);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
    Task AddPaymentAsync(int transactionId, CreatePaymentDto dto);
    Task<IEnumerable<TransactionDto>> GetByClientIdAsync(int clientId);
    Task<decimal> GetRemainingBalanceAsync(int transactionId);
}
