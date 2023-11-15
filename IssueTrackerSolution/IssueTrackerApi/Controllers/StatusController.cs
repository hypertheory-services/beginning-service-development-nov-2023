using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers;

public class StatusController : ControllerBase
{



    [HttpGet("/status")]
    public async Task<ActionResult> GetTheStatus()
    {

        return Ok("Howdy. Looks Good ");
    }
}
