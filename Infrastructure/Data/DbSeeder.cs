using Microsoft.EntityFrameworkCore;
using BudgetTracker.Models;

namespace BudgetTracker.Infrastructure.Data;

public class DbSeeder
{
    private readonly ApplicationDbContext _db;
    public DbSeeder(ApplicationDbContext db) => _db = db;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await _db.Database.MigrateAsync(ct);

        if (!await _db.Users.AnyAsync(ct))
        {
            var alice = new User
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Email = "alice@mail",
                CreatedAt = DateTimeOffset.UtcNow
            };

            var bob = new User
            {
                Id = Guid.NewGuid(),
                Name = "Bob",
                Email = "bob@mail",
                CreatedAt = DateTimeOffset.UtcNow
            };

            _db.Users.AddRange(alice, bob);
            await _db.SaveChangesAsync(ct);

            _db.Transactions.AddRange(
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = alice.Id,
                    Description = "Salary",
                    Amount = 4000,
                    Type = TransactionType.Income,
                    Timestamp = DateTime.UtcNow,
                    Date = DateTime.UtcNow.Date
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = bob.Id,
                    Description = "Rent",
                    Amount = 3000,
                    Type = TransactionType.Expense,
                    Timestamp = DateTime.UtcNow,
                    Date = DateTime.UtcNow.Date
                }
            );

            await _db.SaveChangesAsync(ct);
        }
    }
}
