using Microsoft.Extensions.Logging;
using Quartz;

namespace QuartzNET;

public class LogBackgroundJob : IJob
{
    private readonly ILogger _logger;

    public LogBackgroundJob(ILogger<LogBackgroundJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Logging jon execute on {DateTime.UtcNow}");

        return Task.CompletedTask;
    }
}