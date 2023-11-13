using System.Net;

namespace BusinessClockApi.ContractTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // setup - "Given"
        int a = 10; int b = 20; int answer;
        // when
        answer = a + b; // can .net add two integers.
        // then

        Assert.Equal(30, answer);
    }

    //[Fact]
    //public async Task CanGetTheStatus()
    //{
    //    var client = new HttpClient();
    //    client.BaseAddress = new Uri("http://localhost:1337");

    //    var response = await client.GetAsync("/status");

    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //}
}