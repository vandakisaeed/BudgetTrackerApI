using BudgetTracker.Events;
using BudgetTracker.Services;

namespace BudgetTracker.Services;

public class LoggerService
{
    private readonly string _logFilePath;

    public LoggerService(string logsDirectory, ITransactionService txService)
    {
        if (!Directory.Exists(logsDirectory))
            Directory.CreateDirectory(logsDirectory);

        _logFilePath = Path.Combine(logsDirectory, "transactions.log");

        txService.TransactionAdded += OnTransactionAdded;
    }

    private void OnTransactionAdded(object? sender, TransactionAddedEventArgs e)
    {
        var line = $"{DateTime.UtcNow:u} | {e.Transaction.Type} | {e.Transaction.Amount:C} | {e.Transaction.Description} | Id={e.Transaction.Id}";
        try
        {
            File.AppendAllText(_logFilePath, line + Environment.NewLine);
        }
        catch
        {
            // Logging failures should not crash the app; swallow silently or consider writing to console.
            Console.WriteLine("Warning: failed to write to log file.");
        }
    }
}
