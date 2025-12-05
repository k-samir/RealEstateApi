using RealEstateApi.Domain.Exceptions;

namespace RealEstateApi.Domain.Entities;

public class Payment
{
    public int Id { get; private set; }
    public int TransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Payment() { }

    public static Payment Create(int transactionId, decimal amount, string reference, DateTime date)
    {
        if (amount <= 0) throw new DomainException("Amount must be positive");
        if (string.IsNullOrWhiteSpace(reference)) throw new DomainException("Reference is required");

        return new Payment
        {
            TransactionId = transactionId,
            Amount = amount,
            Reference = reference,
            Date = date,
            CreatedAt = DateTime.UtcNow
        };
    }
}
