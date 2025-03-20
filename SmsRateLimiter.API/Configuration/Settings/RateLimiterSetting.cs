namespace SmsRateLimiter.Service.SeedWorks;

public class RateLimiterSetting : IRateLimiterSetting
{
    public int PhoneNumberLimit { get; set; }
    public int AccountLimit { get; set; }
    public TimeSpan WindowSize { get; set; }
}
