namespace SmsRateLimiter.Service
{
    public interface ISmsRateLimiterService
    {
        bool IsItPossibleToSend(string phoneNumber);
        void CleanupInactiveNumbers();
        int GetAccountLogsPerTime();
        int GetAccountLogs(DateTime startDate, DateTime endDate);
        int GetPhoneLogsPerTime(string phoneNumber);
        int GetPhoneLogs(string phoneNumber, DateTime startDate, DateTime endDate);
    }
}