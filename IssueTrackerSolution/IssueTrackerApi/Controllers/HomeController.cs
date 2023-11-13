using IssueTrackerApi.Models;
using IssueTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers;

public class HomeController : ControllerBase
{

    private readonly BusinessClockApiAdapter _businessClockApiAdapter;
    private readonly ILogger<HomeController> _logger;
    private readonly ISystemTime _systemTime;


    public HomeController(
        BusinessClockApiAdapter businessClockApiAdapter,
        ILogger<HomeController> logger,
        ISystemTime systemTime)
    {
        _businessClockApiAdapter = businessClockApiAdapter;
        _logger = logger;
        _systemTime = systemTime;
    }

    [HttpGet("/")]
    public async Task<ActionResult> GetHomeInfo(CancellationToken ct)
    {

       
       

        _logger.LogInformation("Getting some info!");

        await Task.Delay(1000, ct); // Note: Don't Do This. Classroom only.
        var response = new InfoModelResponse
        {
            CheckedAt = _systemTime.GetCurrent(),
            Description = "Fake Version - Issues API",
            OwnedBy = "Company.com",

        };
        response.Links.Add("/issues", new InfoLink { Description = "Get info about issues, etc." });
        response.Links.Add("/", new InfoLink { Description = "This Document" });

        try
        {
            var contactInfo = await _businessClockApiAdapter.GetStatusAsync(ct);
            response.Support.Email = contactInfo.SupportContact.Email;
            response.Support.Phone = contactInfo.SupportContact.Phone;
            _logger.LogInformation("Successfully got the data from the clock api");
            return Ok(response);
        }
        catch (Exception)
        {
            _logger.LogWarning("Failed to get the data from the clock api - it's busted. CALL NICK!");
            response.Support = null!;
            return Ok(response);
        }



    }
}
