using SmsRateLimiter.Service.SeedWorks;
using System.Collections.Concurrent;

namespace SmsRateLimiter.Service.RateLimiterStrategies;

public class SlidingWindowLogRateLimiterStrategy : IRateLimiterStrategy
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> perNumberLogs = new();
    private readonly ConcurrentQueue<DateTime> accountLog = new();
    private readonly int phoneNumberLimit;
    private readonly int accountLimit;
    private readonly TimeSpan windowSize;
    private readonly object accountLock = new(); // Lock for account-wide log

    public SlidingWindowLogRateLimiterStrategy(IRateLimiterSetting setting)
    {
        phoneNumberLimit = setting.PhoneNumberLimit;
        accountLimit = setting.AccountLimit;
        windowSize = setting.WindowSize;
    }

    public (bool,DateTime?) IsSendMessageValid(string phoneNumber)
    {
        var now = DateTime.UtcNow;

        var numberLog = perNumberLogs.GetOrAdd(phoneNumber, _ => new ConcurrentQueue<DateTime>());
        lock (numberLog)
        {
            if (!IsRequestAcceptable(numberLog, now, phoneNumberLimit))
                return (false, null);
            numberLog.Enqueue(now);
        }
        lock (accountLock)
        {
            if (!IsRequestAcceptable(accountLog, now, accountLimit))
            {
                numberLog.TryDequeue(out _);
                return (false, null);
            }
            accountLog.Enqueue(now);
        }
        return (true, now);
    }

    private bool IsRequestAcceptable(ConcurrentQueue<DateTime> log, DateTime now, int limit)
    {
        // Remove old timestamps outside the sliding window
        while (log.TryPeek(out var timestamp) && now - timestamp > windowSize)
        {
            log.TryDequeue(out _); // Discard the old timestamp
        }

        if (log.Count >= limit)
            return false;

        return true;
    }

    public void CleanupInactiveNumbers()
    {
        var now = DateTime.UtcNow;               
        foreach (var number in perNumberLogs)
        {
            var phoneNumber = number.Key;
            var numberLogs = number.Value;

            // Perform cleanup on the phone number's log
            lock (numberLogs)
            {                
                while (numberLogs.TryPeek(out var timestamp) && now - timestamp > windowSize)
                {
                    numberLogs.TryDequeue(out _);
                }
                if (numberLogs.IsEmpty)
                {
                    perNumberLogs.TryRemove(phoneNumber, out _);
                }
            }
        }
    }
}
