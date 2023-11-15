using System.ComponentModel.DataAnnotations;

namespace IssueTrackerApi.Models;

public record IssueCreateModel
{
    [Required, MinLength(3), MaxLength(200)]
    public string Description { get; init; } = string.Empty;
}

public record IssueResponseModel
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTimeOffset Filed { get; set; }
    public IssuePriority Priority { get; set; }
}

public enum IssuePriority { Question, Bug, FeatureRequest, HighPriority }
