namespace BudgetTracker.Models
{
    public enum TransactionType
    {
        Income,
        Expense
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        // Date-only value used for file partitioning (date component of Timestamp)
        public DateTime Date { get; set; }

        public User? User { get; set; }
    }
}
