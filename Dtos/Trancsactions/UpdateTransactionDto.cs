using BudgetTracker.Models;

namespace BudgetTracker.Dtos.Transactions;

public record UpdateTransactionDto(string? Description, decimal? Amount, TransactionType? Type);