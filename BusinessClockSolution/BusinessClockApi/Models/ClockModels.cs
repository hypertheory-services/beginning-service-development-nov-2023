
namespace BusinessClockApi.Models;

public record ClockResponseModel
{
    public bool IsOpen { get; set; }
    public SupportContactResponseModel SupportContact { get; set; } = new();
    
}
public record SupportContactResponseModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

