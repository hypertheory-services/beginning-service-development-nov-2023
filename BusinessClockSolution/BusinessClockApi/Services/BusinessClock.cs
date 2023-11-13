using BusinessClockApi.Services;

namespace BusinessClockApi;

public class BusinessClock
{

    private readonly ISystemTime _systemTime;

    public BusinessClock(ISystemTime systemTime)
    {
        _systemTime = systemTime;
    }

    public virtual bool IsOpen()
    {
        var now = _systemTime.GetCurrent(); // "Impure"

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