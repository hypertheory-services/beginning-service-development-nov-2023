using IssueTrackerApi.Models;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers;

[ApiController]
public class IssuesController : ControllerBase
{
    private readonly IDocumentSession _session;

    public IssuesController(IDocumentSession session)
    {
        _session = session;
    }

    [HttpPost("/software/{softwareId}/issues/high-priority-issues")]

    public async Task<ActionResult> AddIssueAsync([FromBody] IssueCreateModel request)
    {

        // valid at this point!
        // save it to a database,
        // create some kind of response.

        var response = new IssueResponseModel
        {
            Description = request.Description,
            Filed = DateTimeOffset.Now,
            User = "Joe", // identity
            Id = Guid.NewGuid(),
            Priority = IssuePriority.HighPriority
        };

        _session.Store(response);
        await _session.SaveChangesAsync();
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
