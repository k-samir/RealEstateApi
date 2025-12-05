using RealEstateApi.Application.DTOs;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitRepository _unitRepository;

    public TransactionService(
        ITransactionRepository repository, 
        IClientRepository clientRepository,
        IUnitRepository unitRepository)
    {
        _repository = repository;
        _clientRepository = clientRepository;
        _unitRepository = unitRepository;
    }

    public async Task<(IEnumerable<TransactionDto> items, int total)> GetAllAsync(int? clientId, int page, int pageSize)
    {
        var (items, total) = await _repository.GetAllAsync(clientId, page, pageSize);
        
        var dtos = items.Select(MapToDto);
        return (dtos, total);
    }

    public async Task<TransactionDto?> GetByIdAsync(int id)
    {
        var t = await _repository.GetByIdAsync(id);
        if (t == null) return null;
        return MapToDto(t);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (client == null) throw new DomainException($"Client {dto.ClientId} not found");

        var unit = await _unitRepository.GetByIdAsync(dto.UnitId);
        if (unit == null) throw new DomainException($"Unit {dto.UnitId} not found");

        if (unit.Status != Domain.Enums.UnitStatus.Available)
        {
            throw new DomainException($"Unit {unit.UnitNumber} is not available (Status: {unit.Status})");
        }

        var transaction = Transaction.Create(dto.ClientId, dto.UnitId, dto.Type, dto.DeclaredAmount, dto.UndeclaredAmount, dto.Date);
        await _repository.CreateAsync(transaction);

        if (dto.Type == Domain.Enums.TransactionType.Sale)
        {
            unit.MarkAsSold();
        } 
        else if (dto.Type == Domain.Enums.TransactionType.Rent)
        {
             unit.MarkAsRented();
        }
        
        await _unitRepository.UpdateAsync(unit);

        return MapToDto(transaction);
    }

    public async Task AddPaymentAsync(int transactionId, CreatePaymentDto dto)
    {
        var transaction = await _repository.GetByIdAsync(transactionId);
        if (transaction == null) throw new DomainException($"Transaction {transactionId} not found");

        transaction.AddPayment(dto.Amount, dto.Reference, dto.Date);
        await _repository.UpdateAsync(transaction);
    }

    public async Task<IEnumerable<TransactionDto>> GetByClientIdAsync(int clientId)
    {
        var items = await _repository.GetByClientIdAsync(clientId);
        return items.Select(MapToDto);
    }
    
    public async Task<decimal> GetRemainingBalanceAsync(int transactionId)
    {
         var transaction = await _repository.GetByIdAsync(transactionId);
         if (transaction == null) throw new DomainException($"Transaction {transactionId} not found");
         return transaction.RemainingBalance;
    }

    private TransactionDto MapToDto(Transaction t)
    {
        return new TransactionDto(
            t.Id,
            t.ClientId,
            t.Client?.FullName ?? "Unknown",
            t.UnitId,
            t.Unit?.UnitNumber ?? "Unknown",
            t.Type,
            t.DeclaredAmount,
            t.UndeclaredAmount,
            t.TotalPaid,
            t.RemainingBalance,
            t.Payments.Select(p => new PaymentDto(p.Id, p.Amount, p.Reference, p.Date, p.CreatedAt)).ToList(),
            t.Date,
            t.Status,
            t.CreatedAt
        );
    }
}
