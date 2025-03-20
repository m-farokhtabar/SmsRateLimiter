using SmsRateLimiter.Service.RateLimiterStrategies;
using SmsRateLimiter.Service.SeedWorks;
using System.Collections.Concurrent;

namespace SmsRateLimiter.Service;

public class LocalMemorySmsRateLimiterService : ISmsRateLimiterService
{
    private readonly IRateLimiterStrategy rateLimiterStrategy;
    private readonly ConcurrentDictionary<string, List<DateTime>> logPhone = new();
    private readonly ConcurrentBag<DateTime> logAccount = new();
    private readonly TimeSpan period;
    public LocalMemorySmsRateLimiterService(IRateLimiterStrategy rateLimiterStrategy, IRateLimiterSetting setting)
    {
        this.rateLimiterStrategy = rateLimiterStrategy;
        period = setting.WindowSize;
    }

    public void CleanupInactiveNumbers() => rateLimiterStrategy.CleanupInactiveNumbers();
    public bool IsItPossibleToSend(string phoneNumber)
    {
        (bool status,DateTime? date) = rateLimiterStrategy.IsSendMessageValid(phoneNumber);
        //Instead of using memory we have to use database or external system for keeping all successful data 
        if (status)
        {
            logAccount.Add(date!.Value);
            var numberLog = logPhone.GetOrAdd(phoneNumber, _ => []);
            numberLog.Add(date!.Value);
        }
        return status;
    }
    public int GetAccountLogsPerTime()
    {
        var now = DateTime.UtcNow;
        return logAccount.Count(x => now - x <= period);
    }
    public int GetAccountLogs(DateTime startDate, DateTime endDate)
    {
        return logAccount.Count(x => x >= startDate && x <= endDate);
    }
    public int GetPhoneLogsPerTime(string phoneNumber)
    {
        var now = DateTime.UtcNow;
        var list = logPhone.GetValueOrDefault(phoneNumber);
        if (list?.Count > 0)
            return list.Count(x => now - x <= period);
        else
            return 0;
    }
    public int GetPhoneLogs(string phoneNumber, DateTime startDate, DateTime endDate)
    {        
        var list = logPhone.GetValueOrDefault(phoneNumber);
        if (list?.Count > 0)
            return list.Count(x => x >= startDate && x <= endDate);
        else
            return 0;
    }
}
