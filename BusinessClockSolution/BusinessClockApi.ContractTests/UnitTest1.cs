namespace BusinessClockApi.ContractTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Given
        int a = 11, b = 20;

        // When
        var answer = a + b;

        // Then
        Assert.Equal(31, answer);
    }

    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(10, 2, 12)]
    public void CanAddAnyTwoNumbers(int a, int b, int expecting)
    {
        var answer = a + b;

        Assert.Equal(expecting, answer);
    }
}