namespace IssueTrackerApi.Services;

public class ClockApiAdapter
{
    private readonly HttpClient _httpClient;

    public ClockApiAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // issue-tracker/oncall-developer

    public async Task<OnCallDeveloperApiResponse> GetOnCallDeveloperAsync()
    {
        var response = await _httpClient.GetAsync("/issue-tracker/oncall-developer");

        response.EnsureSuccessStatusCode(); // if it isn't a 200-299 

        var content = await response.Content.ReadFromJsonAsync<OnCallDeveloperApiResponse>();
        if (content is null)
        {
            throw new InvalidOperationException("you will have to deal with this.. more later");
        }
        return content;

    }
}


public record OnCallDeveloperApiResponse(string Name, string EmailAddress, string PhoneNumber);