using Moq;
using SmsRateLimiter.Service;
using SmsRateLimiter.Service.RateLimiterStrategies;
using SmsRateLimiter.Service.SeedWorks;

namespace SmsRateLimiter.Test;

public class LocalMemoryWithWindowSlideRateLimiterTests
{
    [Fact]
    public void When_AllRequestsWithInLimit_Expect_Success()
    {
        var mockRateLimiterSetting = new Mock<IRateLimiterSetting>();
        mockRateLimiterSetting.SetupGet(x => x.PhoneNumberLimit).Returns(5);
        mockRateLimiterSetting.SetupGet(x => x.AccountLimit).Returns(10);
        mockRateLimiterSetting.SetupGet(x => x.WindowSize).Returns(TimeSpan.FromSeconds(1));
        var service = new LocalMemorySmsRateLimiterService(new SlidingWindowLogRateLimiterStrategy(mockRateLimiterSetting.Object), mockRateLimiterSetting.Object);
        string[] Numbers = ["1", "1", "2", "2", "3", "1", "1", "2", "1","3"];
        
        foreach (var number in Numbers)
            Assert.True(service.IsItPossibleToSend(number));
    }

    [Fact]
    public void When_ExceedsPhoneNumberLimit_Expect_DenyThePhoneNumber()
    {
        var mockRateLimiterSetting = new Mock<IRateLimiterSetting>();
        mockRateLimiterSetting.SetupGet(x => x.PhoneNumberLimit).Returns(1);
        mockRateLimiterSetting.SetupGet(x => x.AccountLimit).Returns(10);
        mockRateLimiterSetting.SetupGet(x => x.WindowSize).Returns(TimeSpan.FromSeconds(1));
        var service = new LocalMemorySmsRateLimiterService(new SlidingWindowLogRateLimiterStrategy(mockRateLimiterSetting.Object), mockRateLimiterSetting.Object);
        
        service.IsItPossibleToSend("1");
        Assert.False(service.IsItPossibleToSend("1"));
    }

    [Fact]
    public void When_ExceedsAccountLimit_Expect_DenyRequests()
    {
        var mockRateLimiterSetting = new Mock<IRateLimiterSetting>();
        mockRateLimiterSetting.SetupGet(x => x.PhoneNumberLimit).Returns(10);
        mockRateLimiterSetting.SetupGet(x => x.AccountLimit).Returns(1);
        mockRateLimiterSetting.SetupGet(x => x.WindowSize).Returns(TimeSpan.FromSeconds(1));
        var service = new LocalMemorySmsRateLimiterService(new SlidingWindowLogRateLimiterStrategy(mockRateLimiterSetting.Object), mockRateLimiterSetting.Object);

        service.IsItPossibleToSend("1");
        Assert.False(service.IsItPossibleToSend("2"));
    }

    [Fact]
    public void When_RateLimiterReset_Expect_AllowRequestsAgain()
    {
        var mockRateLimiterSetting = new Mock<IRateLimiterSetting>();
        mockRateLimiterSetting.SetupGet(x => x.PhoneNumberLimit).Returns(10);
        mockRateLimiterSetting.SetupGet(x => x.AccountLimit).Returns(10);
        mockRateLimiterSetting.SetupGet(x => x.WindowSize).Returns(TimeSpan.FromSeconds(1));
        var service = new LocalMemorySmsRateLimiterService(new SlidingWindowLogRateLimiterStrategy(mockRateLimiterSetting.Object), mockRateLimiterSetting.Object);
        
        for(int i=0;i<10;i++)
            Assert.True(service.IsItPossibleToSend(Random.Shared.Next(20).ToString()));
        
        Assert.False(service.IsItPossibleToSend(Random.Shared.Next(20).ToString()));
        Thread.Sleep(1000);
        Assert.True(service.IsItPossibleToSend(Random.Shared.Next(20).ToString()));
    }

    [Fact]
    public void When_ConcurrentRequests_Expect_RespectsLimits()
    {

        var maxPhoneNumberLimit = 10;
        var maxAccountLimit = 100;

        var mockRateLimiterSetting = new Mock<IRateLimiterSetting>();
        mockRateLimiterSetting.SetupGet(x => x.PhoneNumberLimit).Returns(maxPhoneNumberLimit);
        mockRateLimiterSetting.SetupGet(x => x.AccountLimit).Returns(maxAccountLimit);
        mockRateLimiterSetting.SetupGet(x => x.WindowSize).Returns(TimeSpan.FromSeconds(1));
        var service = new LocalMemorySmsRateLimiterService(new SlidingWindowLogRateLimiterStrategy(mockRateLimiterSetting.Object), mockRateLimiterSetting.Object);

        var businessPhoneNumber = "1";
        var totalRequests = 500;
        var successfulRequests = 0;

        Parallel.ForEach(Enumerable.Range(0, totalRequests), (i) =>
        {
            if (service.IsItPossibleToSend(businessPhoneNumber))
                Interlocked.Increment(ref successfulRequests);
        });

        Assert.True(successfulRequests == maxPhoneNumberLimit,
            $"Expected at most {maxPhoneNumberLimit} successful requests, but got {successfulRequests}.");
    }
}