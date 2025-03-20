namespace SmsRateLimiter.Service.SeedWorks;

public interface IRateLimiterSetting
{
    int PhoneNumberLimit { get; }
    int AccountLimit { get; }
    TimeSpan WindowSize { get; }
}
