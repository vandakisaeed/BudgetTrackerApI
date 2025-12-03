#region transaction with storage
// using BudgetTracker.Events;
// using BudgetTracker.Models;

// namespace BudgetTracker.Services;

// public class TransactionService : ITransactionService
// {
//     private readonly StorageService _storage;

//     public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;

//     public TransactionService(StorageService storage)
//     {
//         _storage = storage;
//     }

//     public Task<IReadOnlyList<Transaction>> ListAsync()
//     {
//         var all = _storage.GetAllTransactions().ToList();
//         return Task.FromResult((IReadOnlyList<Transaction>)all);
//     }

//     public Task<IReadOnlyList<Transaction>> ListByUserAsync(Guid userId)
//     {
//         var list = _storage.GetAllTransactions().Where(t => t.UserId == userId).ToList();
//         return Task.FromResult((IReadOnlyList<Transaction>)list);
//     }

//     public Task<Transaction?> GetAsync(Guid id)
//     {
//         var tx = _storage.GetAllTransactions().FirstOrDefault(t => t.Id == id);
//         return Task.FromResult(tx);
//     }

//     public async Task<Transaction> CreateAsync(Guid userId, string description, decimal amount, TransactionType type, DateTime? timestamp = null)
//     {
//         var now = timestamp ?? DateTime.Now;
//         var tx = new Transaction
//         {
//             Id = Guid.NewGuid(),
//             UserId = userId,
//             Timestamp = now,
//             Date = now.Date,
//             Type = type,
//             Description = description,
//             Amount = amount
//         };

//         _storage.SaveTransaction(tx);

//         TransactionAdded?.Invoke(this, new TransactionAddedEventArgs(tx));

//         return tx;
//     }

//     public Task<Transaction?> UpdateAsync(Guid id, string? description, decimal? amount, TransactionType? type)
//     {
//         var tx = _storage.GetAllTransactions().FirstOrDefault(t => t.Id == id);
//         if (tx == null) return Task.FromResult<Transaction?>(null);

//         if (description is not null) tx.Description = description;
//         if (amount is not null) tx.Amount = amount.Value;
//         if (type is not null) tx.Type = type.Value;

//         var updated = _storage.UpdateTransaction(tx);
//         return updated ? Task.FromResult<Transaction?>(tx) : Task.FromResult<Transaction?>(null);
//     }

//     public Task<bool> DeleteAsync(Guid id)
//     {
//         return Task.FromResult(_storage.DeleteTransaction(id));
//     }
// }

#endregion

#region transaction with db
using BudgetTracker.Events;
using BudgetTracker.Models;
using BudgetTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Services;

// transaction with db
public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _db;

    public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;

    public TransactionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Transaction>> ListAsync()
    {
        return await _db.Transactions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Transaction>> ListByUserAsync(Guid userId)
    {
        return await _db.Transactions
            .Where(t => t.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Transaction?> GetAsync(Guid id)
    {
        return await _db.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> CreateAsync(Guid userId, string description,
        decimal amount, TransactionType type, DateTime? timestamp = null)
    {
        var now = timestamp ?? DateTime.UtcNow;

        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Timestamp = now,
            Date = now.Date,
            Type = type,
            Description = description,
            Amount = amount
        };

        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();

        TransactionAdded?.Invoke(this, new TransactionAddedEventArgs(tx));

        return tx;
    }

    public async Task<Transaction?> UpdateAsync(Guid id, string? description,
        decimal? amount, TransactionType? type)
    {
        var tx = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (tx == null) return null;

        if (description != null) tx.Description = description;
        if (amount != null) tx.Amount = amount.Value;
        if (type != null) tx.Type = type.Value;

        await _db.SaveChangesAsync();
        return tx;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tx = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (tx == null) return false;

        _db.Transactions.Remove(tx);
        await _db.SaveChangesAsync();
        return true;
    }
}

#endregion

