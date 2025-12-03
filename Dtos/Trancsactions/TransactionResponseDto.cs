using BudgetTracker.Models;

namespace BudgetTracker.Dtos.Transactions;

public record TransactionResponseDto(
    Guid Id,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateTime Timestamp,
    DateTime Date
);
