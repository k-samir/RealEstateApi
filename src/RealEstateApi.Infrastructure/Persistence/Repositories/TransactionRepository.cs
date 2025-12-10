using Microsoft.EntityFrameworkCore;
using RealEstateApi.Application.Interfaces;
using RealEstateApi.Domain.Entities;
using RealEstateApi.Domain.Enums;

namespace RealEstateApi.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Transaction> items, int total)> GetAllAsync(int? clientId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Transactions
            .Include(t => t.Client)
            .Include(t => t.Payments) 
            .AsQueryable();

        if (clientId.HasValue)
        {
            query = query.Where(t => t.ClientId == clientId.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Client)
            .Include(t => t.Payments)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasActiveTransactionForUnitAsync(string unitId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AnyAsync(t => t.UnitId == unitId &&
                          (t.Status == TransactionStatus.Pending || t.Status == TransactionStatus.Completed),
                          cancellationToken);
    }

    public async Task<bool> HasTransactionsForUnitAsync(string unitId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AnyAsync(t => t.UnitId == unitId, cancellationToken);
    }

    public async Task<List<Transaction>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Payments)
            .Where(t => t.ClientId == clientId)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }
}
