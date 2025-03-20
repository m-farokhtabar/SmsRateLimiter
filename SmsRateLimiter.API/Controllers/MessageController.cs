using Microsoft.AspNetCore.Mvc;
using SmsRateLimiter.Service;
using System.ComponentModel.DataAnnotations;

namespace SmsRateLimiter.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    public readonly ISmsRateLimiterService smsRateLimiterService;    
    private readonly ILogger<MessageController> logger;

    public MessageController(ILogger<MessageController> logger, ISmsRateLimiterService smsRateLimiterService)
    {
        this.logger = logger;
        this.smsRateLimiterService = smsRateLimiterService;
    }

    [HttpPost("Send/{phoneNumber}")]
    public IActionResult Send([Required(ErrorMessage = "Phone number is required")] string phoneNumber)
    {
        return smsRateLimiterService.IsItPossibleToSend(phoneNumber) ? Ok() : StatusCode(StatusCodes.Status429TooManyRequests);
    }

    [HttpGet("status/accountlog")]
    public IActionResult GetAccountLogsPerTime() => Ok(smsRateLimiterService.GetAccountLogsPerTime());

    [HttpGet("status/phonelog/{phoneNumber}")]
    public IActionResult GetPhoneLogsPerTime([Required(ErrorMessage = "Phone number is required")] string phoneNumber) => Ok(smsRateLimiterService.GetPhoneLogsPerTime(phoneNumber));

    [HttpGet("status/phonelog/{phoneNumber}/{from:datetime}/{to:datetime}")]
    public IActionResult GetPhoneLogsByDate([Required(ErrorMessage = "Phone number is required")] string phoneNumber, DateTime from,DateTime to) => Ok(smsRateLimiterService.GetPhoneLogs(phoneNumber,from,to));

}
