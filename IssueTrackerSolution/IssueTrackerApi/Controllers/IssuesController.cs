using IssueTrackerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers;

[ApiController]
public class IssuesController : ControllerBase
{
    [HttpPost("/software/{softwareId}/issues/high-priority-issues")]

    public async Task<ActionResult> AddIssueAsync([FromBody] IssueCreateModel request)
    {

        // valid at this point!
        // save it to a database,
        // create some kind of response.

        return Ok();
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
