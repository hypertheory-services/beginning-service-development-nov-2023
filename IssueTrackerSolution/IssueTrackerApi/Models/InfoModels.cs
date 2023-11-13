namespace IssueTrackerApi.Models;


public record InfoModelResponse
{
	public string Description { get; set; } = string.Empty;
	public string OwnedBy { get; set; } = string.Empty;
	public DateTime CheckedAt { get; set; }

	public Dictionary<string, InfoLink> Links { get; set; } = new();

	public SupportInfo Support { get; set; } = new();

}

public record SupportInfo
{
	public string Email { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
}

public record InfoLink
{
	public string Description { get; set; } = string.Empty;
}
/*{
	"description": "Issue Tracking API",
	"ownedBy": "Company.Com",
	"checkedAt": "2023....",
	"links": {
		"/issues": {
			"description": "Add or see issues"
		}
	},
	"support": {
		"email": "mitch@company.com",
		"phone": "888 888-1212"
	}

}*/


/*
 * {
    "isOpen": true,
    "supportContact": {
        "name": "Mitchell",
        "email": "mitch@company.com",
        "phone": "800 555-1212"
    }
}
*/

public record StatusApiResponseModel
{
	public SupportContactFromStatusApiModel SupportContact { get; set; } = new();
}

public record SupportContactFromStatusApiModel
{
	public string Email { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
}
