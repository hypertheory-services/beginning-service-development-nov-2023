namespace BusinessClockApi;

public class StandardBusinessClock : IProvideTheBusinessClock
{
    private ISystemTime _clock;

    public StandardBusinessClock(ISystemTime clock)
    {
        _clock = clock;
    }

    public bool IsOpen()
    {
        var now = _clock.GetCurrent();

        var dayOfWeek = now.DayOfWeek;

        var hour = now.Hour;
        var openingTime = new TimeSpan(9, 0, 0);
        var closingTime = new TimeSpan(17, 0, 0);

        var isOpen = dayOfWeek switch
        {
            DayOfWeek.Sunday => false,
            DayOfWeek.Saturday => false,
            _ => hour >= openingTime.Hours && hour < closingTime.Hours,
        };

        return isOpen;
    }
}

public interface ISystemTime
{
    DateTime GetCurrent();
}

public class SystemTime : ISystemTime
{
    public DateTime GetCurrent() => DateTime.Now;
}

