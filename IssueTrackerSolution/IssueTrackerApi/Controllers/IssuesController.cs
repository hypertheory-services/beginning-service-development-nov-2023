using IssueTrackerApi.Models;
using IssueTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers;

[ApiController]
public class IssuesController : ControllerBase
{
    private readonly IssuesCatalog _catalog;

    public IssuesController(IssuesCatalog catalog)
    {
        _catalog = catalog;
    }

    [HttpPost("/software/{softwareId}/issues/high-priority-issues")]

    public async Task<ActionResult> AddIssueAsync([FromBody] IssueCreateModel request)
    {

        var user = "Joe";
        IssueResponseModel response = await _catalog.FileIssueAsync(request, user, IssuePriority.HighPriority);
        return Ok(response);
    }


    [HttpGet("/software")]
    public async Task<ActionResult> GetSoftwareCatalog()
    {
        var catalog = new List<string>
        {
            "excel",
            "powerpoint",
            "vscode"
        };
        return Ok(catalog);
    }
}
