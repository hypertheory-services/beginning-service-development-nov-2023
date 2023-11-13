

using BusinessClockApi.Services;
using NSubstitute;

namespace BusinessClockApi.UnitTests;

public class BusinessClockTests
{
    [Fact]
    public void ClosedOnSaturday()
    {
        // Given

        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(new DateTime(2023, 8, 26));
        var clock = new BusinessClock(stubbedSystemTime);

        // When
        bool isOpen = clock.IsOpen();
        // Then
        Assert.False(isOpen);
    }

    [Fact]
    public void ClosedOnSunday()
    {

        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(new DateTime(2023, 8, 27));
        var clock = new BusinessClock(stubbedSystemTime);

        bool isOpen = clock.IsOpen();

        Assert.False(isOpen);
    }
    // TODO Added a Case
    [Theory]
    [InlineData("8/21/2023 9:00:00")]
    [InlineData("8/21/2023 16:59:59")]
    [InlineData("8/21/2023 12:59:59")]
    public void WeAreOpen(string dateTime)
    {
        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(DateTime.Parse(dateTime));
        var clock = new BusinessClock(stubbedSystemTime);

        Assert.True(clock.IsOpen());
    }
    // TODO Added A Case
    [Theory]
    [InlineData("8/21/2023 8:59:59")]
    [InlineData("8/21/2023 17:00:00")]
    [InlineData("8/21/2023 23:00:00")]
    public void WeAreClosed(string dateTime)
    {
        var stubbedSystemTime = Substitute.For<ISystemTime>();
        stubbedSystemTime.GetCurrent().Returns(DateTime.Parse(dateTime));
        var clock = new BusinessClock(stubbedSystemTime);

        Assert.False(clock.IsOpen());
    }
}
