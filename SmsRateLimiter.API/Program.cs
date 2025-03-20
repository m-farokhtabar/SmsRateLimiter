using Mapper.GSB.Rest.API.StartupConfig.Applications;
using SmsRateLimiter.API.Services;
using SmsRateLimiter.Service.Configuration;
using SmsRateLimiter.Service.SeedWorks;

var builder = WebApplication.CreateBuilder(args);

RateLimiterSetting rateSettings = builder.Configuration.GetSection("RateLimiterSettings").Get<RateLimiterSetting>()!;
SlidingWindowLogCleanupServiceSetting srvclnSettings = builder.Configuration.GetSection("CleanupServiceSetting").Get<SlidingWindowLogCleanupServiceSetting>()!;
builder.Services.AddSingleton(srvclnSettings);
// Add services to the container.
builder.Services.AddHostedService<SlidingWindowLogCleanupService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.SmsRateLimiterServiceConfiguration(x => x.LocalMemoryWithWindowLogConfiguration(rateSettings));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCorsAllowAny();

app.UseAuthorization();

app.MapControllers();

app.Run();
