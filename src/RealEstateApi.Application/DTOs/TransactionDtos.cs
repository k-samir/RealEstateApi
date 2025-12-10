using RealEstateApi.Domain.Enums;

namespace RealEstateApi.Application.DTOs;

public record TransactionDto(
    int Id,
    string AgentId,
    int ClientId,
    string ClientName,
    string UnitId,
    string UnitNumber,
    TransactionType Type,
    decimal DeclaredAmount,
    decimal UndeclaredAmount,
    decimal TotalPaid,
    decimal RemainingBalance,
    List<PaymentDto> Payments,
    DateTime Date,
    TransactionStatus Status,
    DateTime CreatedAt
);

public record CreateTransactionDto(
    string AgentId,
    int ClientId,
    string UnitId,
    TransactionType Type,
    decimal DeclaredAmount,
    decimal UndeclaredAmount,
    DateTime Date
);

public record PaymentDto(
    int Id,
    decimal Amount,
    string Reference,
    DateTime Date,
    DateTime CreatedAt
);

public record CreatePaymentDto(
    decimal Amount,
    string Reference,
    DateTime Date
);
