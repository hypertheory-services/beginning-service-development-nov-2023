
using NSubstitute;

namespace BusinessClockApi.UnitTests;
public class BusinessClockTests
{
    [Fact]
    public void ClosedOnSaturday()
    {
        var fakeClock = Substitute.For<ISystemTime>();
        fakeClock.GetCurrent().Returns(new DateTime(2023, 11, 18, 9, 15, 00));
        var clock = new StandardBusinessClock(fakeClock);

        bool status = clock.IsOpen(); // System Under Test.

        Assert.False(status);
    }
    [Fact]
    public void ClosedOnSunday()
    {
        var fakeClock = Substitute.For<ISystemTime>();
        fakeClock.GetCurrent().Returns(new DateTime(2023, 11, 19, 9, 15, 00));
        var clock = new StandardBusinessClock(fakeClock);

        var status = clock.IsOpen();

        Assert.False(status);
    }

    [Theory]
    [InlineData("11/13/2023 16:25:00")]
    [InlineData("11/13/2023 9:00:00")]
    public void WeAreOpen(string dateTime)
    {
        var fakeClock = Substitute.For<ISystemTime>();
        fakeClock.GetCurrent().Returns(DateTime.Parse(dateTime));
        var clock = new StandardBusinessClock(fakeClock);

        Assert.True(clock.IsOpen());
    }
    [Theory]
    [InlineData("11/13/2023 8:59:59")]
    [InlineData("11/13/2023 17:00:00")]
    public void WeAreClosed(string dateTime)
    {
        var fakeClock = Substitute.For<ISystemTime>();
        fakeClock.GetCurrent().Returns(DateTime.Parse(dateTime));
        var clock = new StandardBusinessClock(fakeClock);

        Assert.True(clock.IsOpen());
    }
}
