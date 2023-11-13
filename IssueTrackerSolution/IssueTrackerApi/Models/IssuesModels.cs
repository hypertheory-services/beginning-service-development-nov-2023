using System.ComponentModel.DataAnnotations;

namespace IssueTrackerApi.Models;

public record IssueCreateRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;
    public string Application { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
}


public record IssueResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Application { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public IssueStatus Status { get; set; } = IssueStatus.Open;
}

public enum IssueStatus { Open, Closed }