using BudgetTracker.Models;

namespace BudgetTracker.Services;

public interface ITransactionService
{
    event EventHandler<BudgetTracker.Events.TransactionAddedEventArgs>? TransactionAdded;
    Task<IReadOnlyList<Transaction>> ListAsync();
    Task<IReadOnlyList<Transaction>> ListByUserAsync(Guid userId);
    Task<Transaction?> GetAsync(Guid id);
    Task<Transaction> CreateAsync(Guid userId, string description, decimal amount, TransactionType type, DateTime? timestamp = null);
    Task<Transaction?> UpdateAsync(Guid id, string? description, decimal? amount, TransactionType? type);
    Task<bool> DeleteAsync(Guid id);
}
