namespace SmsRateLimiter.Service.RateLimiterStrategies;

public  interface IRateLimiterStrategy
{
    (bool, DateTime?) IsSendMessageValid(string phoneNumber);
    void CleanupInactiveNumbers();
}