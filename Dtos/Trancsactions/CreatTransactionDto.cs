using System.ComponentModel.DataAnnotations;
using BudgetTracker.Models;

namespace BudgetTracker.Dtos.Transactions;

public record CreateTransactionDto(
[property: Required]
Guid UserId,
[property: Required]
string Description,
[property: Required]
decimal Amount,
[property: Required]
TransactionType Type);