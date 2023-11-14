namespace BusinessClockApi.Services;

public class OnCallDeveloperLookup
{
    private readonly IProvideTheBusinessClock _clock;

    public OnCallDeveloperLookup(IProvideTheBusinessClock clock)
    {
        _clock = clock;
    }

    public OnCallDeveloperResponse GetOnCallDeveloper()
    {

        if (_clock.IsOpen())
        {
            return new OnCallDeveloperResponse("Bob Smith", "bob@aol.com", "555-1212");
        }
        else
        {
            return new OnCallDeveloperResponse("Support Pros, Inc.", "support@pros.com", "800 592-1838");
        }
    }
}
