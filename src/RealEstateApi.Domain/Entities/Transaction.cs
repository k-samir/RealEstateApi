using RealEstateApi.Domain.Enums;
using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

public class Transaction
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public Client Client { get; private set; } = null!;
    
    public string UnitId { get; private set; } = string.Empty;
    public Unit Unit { get; private set; } = null!;

    public TransactionType Type { get; private set; }
    public decimal DeclaredAmount { get; private set; } 
    public decimal UndeclaredAmount { get; private set; }
    public DateTime Date { get; private set; }
    public TransactionStatus Status { get; private set; }

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public decimal TotalPaid => _payments.Sum(p => p.Amount);
    public decimal RemainingBalance => UndeclaredAmount - TotalPaid; // Use undeclared amount for balance

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(int clientId, string unitId, TransactionType type, decimal declaredAmount, decimal undeclaredAmount, DateTime date)
    {
        if (declaredAmount <= 0) throw new DomainException("Declared amount must be positive");
        if (undeclaredAmount <= 0) throw new DomainException("Undeclared amount must be positive");
        if (string.IsNullOrWhiteSpace(unitId)) throw new DomainException("Unit is required");

        var now = DateTime.UtcNow;
        return new Transaction
        {
            ClientId = clientId,
            UnitId = unitId,
            Type = type,
            DeclaredAmount = declaredAmount,
            UndeclaredAmount = undeclaredAmount,
            Date = date,
            Status = TransactionStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void AddPayment(decimal amount, string reference, DateTime date)
    {
        if (Status == TransactionStatus.Cancelled) throw new DomainException("Cannot add payment to cancelled transaction");
        if (amount <= 0) throw new DomainException("Payment amount must be positive");
        if (TotalPaid + amount > UndeclaredAmount) throw new DomainException("Payment exceeds remaining balance");

        _payments.Add(Payment.Create(Id, amount, reference, date));
        UpdatedAt = DateTime.UtcNow;
        
        // Auto-complete if paid fully?
        if (RemainingBalance == 0 && Status == TransactionStatus.Pending)
        {
            Status = TransactionStatus.Completed;
        }
    }
    
    public void Cancel()
    {
        if (Status == TransactionStatus.Completed) throw new DomainException("Cannot cancel completed transaction");
        Status = TransactionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
