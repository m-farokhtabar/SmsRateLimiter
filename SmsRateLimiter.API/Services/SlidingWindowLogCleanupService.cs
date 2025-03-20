using SmsRateLimiter.Service;

namespace SmsRateLimiter.API.Services;

public class SlidingWindowLogCleanupService : BackgroundService
{
    private readonly ISmsRateLimiterService smsRateLimiterService;
    public readonly PeriodicTimer timer;

    public SlidingWindowLogCleanupService(ISmsRateLimiterService smsRateLimiterService, SlidingWindowLogCleanupServiceSetting setting)
    {
        this.smsRateLimiterService = smsRateLimiterService;
        timer = new PeriodicTimer(TimeSpan.FromSeconds(setting.Period));
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {                
            smsRateLimiterService.CleanupInactiveNumbers();
        }
    }
}
