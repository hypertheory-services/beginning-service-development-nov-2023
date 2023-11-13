
using Alba;
using BusinessClockApi.Models;
using BusinessClockApi.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace BusinessClockApi.ContractTests.Status;

public class StatusTests
{


   
    // TODO: This is what I wanted to do yesterday.
    [Fact]
    public async Task WhileWeAreClosed2()
    {

        var host = await AlbaHost.For<Program>(config =>
        {
            var time = Substitute.For<ISystemTime>();
            var clock = Substitute.For<BusinessClock>(time);

            clock.IsOpen().Returns(false);

            config.ConfigureServices(services =>
            {
                services.AddSingleton<BusinessClock>(clock);
            });
        });

        var expectedResponse = new ClockResponseModel
        {
            IsOpen = false,
            SupportContact = new SupportContactResponseModel
            {
                Name = "Support Pros Inc.",
                Phone = "800 999-1213x23",
                Email = "support@support-pros.com"
            }
        };

        var response = await host.Scenario(api =>
        {
            api.Get.Url("/status");
            api.StatusCodeShouldBeOk();

        });

        var responseMessage = response.ReadAsJson<ClockResponseModel>();

        Assert.NotNull(responseMessage);

        Assert.Equal(expectedResponse, responseMessage);
    }
    [Fact]
    public async Task WhileWeAreOpen2()
    {

        var host = await AlbaHost.For<Program>(config =>
        {
            var time = Substitute.For<ISystemTime>();
            var clock = Substitute.For<BusinessClock>(time);

            clock.IsOpen().Returns(true);

            config.ConfigureServices(services =>
            {
                services.AddSingleton<BusinessClock>(clock);
            });
        });

        var expectedResponse = new ClockResponseModel
        {
            IsOpen = true,
            SupportContact = new SupportContactResponseModel
            {
                Name = "Susan",
                Phone = "800 555-1212",
                Email = "Susan@company.com"
            }
        };

        var response = await host.Scenario(api =>
        {
            api.Get.Url("/status");
            api.StatusCodeShouldBeOk();

        });

        var responseMessage = response.ReadAsJson<ClockResponseModel>();

        Assert.NotNull(responseMessage);

        Assert.Equal(expectedResponse, responseMessage);
    }
}
