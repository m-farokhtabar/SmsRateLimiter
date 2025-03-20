using Microsoft.Extensions.DependencyInjection;
using SmsRateLimiter.Service.RateLimiterStrategies;
using SmsRateLimiter.Service.SeedWorks;

namespace SmsRateLimiter.Service.Configuration;

public class SmsRateLimiterServiceConfiguration
{
    private readonly IServiceCollection services;

    public SmsRateLimiterServiceConfiguration(IServiceCollection services)
    {
        this.services = services;
    }

    public void LocalMemoryWithWindowLogConfiguration(IRateLimiterSetting setting)
    {
        services.AddSingleton<IRateLimiterSetting>(_ => setting);
        services.AddSingleton<ISmsRateLimiterService, LocalMemorySmsRateLimiterService>();
        services.AddSingleton<IRateLimiterStrategy, SlidingWindowLogRateLimiterStrategy>();
    }
}
