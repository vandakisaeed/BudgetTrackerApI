using System.Text.Json;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

public class StorageService
{
    private readonly string _dataDirectory;

    public StorageService(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
        if (!Directory.Exists(_dataDirectory))
            Directory.CreateDirectory(_dataDirectory);
    }

    private string FilePathForDate(DateTime date)
    {
        var fileName = date.ToString("yyyy-MM-dd") + ".json";
        return Path.Combine(_dataDirectory, fileName);
    }

    public void SaveTransaction(Transaction tx)
    {
        // Ensure Date property set (use local date component of timestamp)
        tx.Date = tx.Timestamp.ToLocalTime().Date;

        var path = FilePathForDate(tx.Date);

        List<Transaction> list = new();
        if (File.Exists(path))
        {
            try
            {
                var text = File.ReadAllText(path);
                list = JsonSerializer.Deserialize<List<Transaction>>(text) ?? new List<Transaction>();
            }
            catch
            {
                // If the file is malformed, overwrite with fresh list
                list = new List<Transaction>();
            }
        }

        list.Add(tx);

        var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public List<Transaction> ReadTransactions(DateTime date)
    {
        var path = FilePathForDate(date.Date);
        if (!File.Exists(path))
            return new List<Transaction>();

        var text = File.ReadAllText(path);
        try
        {
            return JsonSerializer.Deserialize<List<Transaction>>(text) ?? new List<Transaction>();
        }
        catch
        {
            return new List<Transaction>();
        }
    }

    public IEnumerable<Transaction> GetAllTransactions()
    {
        var results = new List<Transaction>();
        var files = Directory.GetFiles(_dataDirectory, "*.json");
        foreach (var f in files)
        {
            try
            {
                var text = File.ReadAllText(f);
                var list = JsonSerializer.Deserialize<List<Transaction>>(text);
                if (list != null && list.Count > 0)
                    results.AddRange(list);
            }
            catch
            {
                // ignore malformed files
            }
        }
        return results;
    }

    public bool UpdateTransaction(Transaction tx)
    {
        var files = Directory.GetFiles(_dataDirectory, "*.json");
        foreach (var f in files)
        {
            try
            {
                var text = File.ReadAllText(f);
                var list = JsonSerializer.Deserialize<List<Transaction>>(text);
                if (list == null || list.Count == 0)
                    continue;

                var idx = list.FindIndex(t => t.Id == tx.Id);
                if (idx >= 0)
                {
                    list[idx] = tx;
                    var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(f, json);
                    return true;
                }
            }
            catch
            {
                // ignore malformed files
            }
        }

        return false;
    }

    public bool DeleteTransaction(Guid id)
    {
        // Scan all files in data directory to find and remove transaction
        var files = Directory.GetFiles(_dataDirectory, "*.json");
        foreach (var f in files)
        {
            try
            {
                var text = File.ReadAllText(f);
                var list = JsonSerializer.Deserialize<List<Transaction>>(text);
                if (list == null || list.Count == 0)
                    continue;

                var origCount = list.Count;
                list.RemoveAll(t => t.Id == id);
                if (list.Count != origCount)
                {
                    // write back (if empty, write empty array)
                    var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(f, json);
                    return true;
                }
            }
            catch
            {
                // ignore malformed files
            }
        }

        return false;
    }

    public IEnumerable<Transaction> GetTransactionsBetween(DateTime startInclusive, DateTime endInclusive)
    {
        var results = new List<Transaction>();
        for (var date = startInclusive.Date; date <= endInclusive.Date; date = date.AddDays(1))
        {
            results.AddRange(ReadTransactions(date));
        }
        return results;
    }
}
