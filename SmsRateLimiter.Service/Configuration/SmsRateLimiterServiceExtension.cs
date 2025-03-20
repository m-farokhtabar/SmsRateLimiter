using Microsoft.Extensions.DependencyInjection;

namespace SmsRateLimiter.Service.Configuration;

public static class SmsRateLimiterServiceExtension
{
    public static void SmsRateLimiterServiceConfiguration(this IServiceCollection services, Action<SmsRateLimiterServiceConfiguration> configuration)
    {
        var infraCfg = new SmsRateLimiterServiceConfiguration(services);
        configuration.Invoke(infraCfg);
    }
}
