namespace IssueTrackerApi.Services;

public interface ISystemTime
{
    DateTime GetCurrent();
}

public class SystemTime : ISystemTime
{
    private readonly ILogger<SystemTime> _logger;

    public SystemTime(ILogger<SystemTime> logger)
    {
        _logger = logger;
        _logger.LogInformation("Created the SystemTime from the Factory");
    }

    public DateTime GetCurrent() => DateTime.Now;
}
