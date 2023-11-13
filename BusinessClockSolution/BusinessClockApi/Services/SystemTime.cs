namespace BusinessClockApi.Services;

public interface ISystemTime
{
    DateTime GetCurrent();
}

public class SystemTime : ISystemTime
{
    public DateTime GetCurrent()
    {
        return DateTime.Now;
    }
}
