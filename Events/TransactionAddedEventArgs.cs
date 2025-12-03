using System;
using BudgetTracker.Models;

namespace BudgetTracker.Events;

public class TransactionAddedEventArgs : EventArgs
{
    public Transaction Transaction { get; }

    public TransactionAddedEventArgs(Transaction transaction)
    {
        Transaction = transaction;
    }
}
