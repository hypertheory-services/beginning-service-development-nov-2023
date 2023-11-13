using IssueTrackerApi.Models;

namespace IssueTrackerApi.Services;

// For each external API you call (remote address), create one of these.
// Do not use the same adapter for multiple remote servers.
public class BusinessClockApiAdapter
{
    private readonly HttpClient _httpClient;

    public BusinessClockApiAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StatusApiResponseModel> GetStatusAsync(CancellationToken ct)
    {
        var response = await _httpClient.GetAsync("/status", ct);

        response.EnsureSuccessStatusCode(); // Weird. If this is not a 200-299 response, punch me in the nose.

        var content = await response.Content.ReadFromJsonAsync<StatusApiResponseModel?>();

        if(content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }
        return content;
    }
}
