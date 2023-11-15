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
    [HttpPost("/software/{softwareId}/issues/question")]
    public async Task<ActionResult> AddQuestion([FromBody] IssueCreateModel request)
    {

        var user = "Joe";
        IssueResponseModel response = await _catalog.FileIssueAsync(request, user, IssuePriority.Question);
        return Ok(response);
    }

    [HttpGet("/issues/")]
    public async Task<ActionResult> GetAllIssues()
    {
        var issues = await _catalog.GetAllIssuesAsync();
        return Ok(new { issues });
    }

    // public enum IssuePriority { Question, Bug, FeatureRequest, HighPriority }

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
