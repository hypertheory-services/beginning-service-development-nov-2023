using IssueTrackerApi.Models;
using Marten;

namespace IssueTrackerApi.Services;

public class IssuesCatalog
{
    private readonly IDocumentSession _session;

    public IssuesCatalog(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IssueResponseModel> FileIssueAsync(IssueCreateModel request, string user, IssuePriority priority)
    {
        var response = new IssueResponseModel
        {
            Description = request.Description,
            Filed = DateTimeOffset.Now,
            Id = Guid.NewGuid(),
            Priority = priority,
            User = user
        };
        _session.Store(response);
        await _session.SaveChangesAsync();
        return response;
    }

    public async Task<IReadOnlyList<IssueResponseModel>> GetAllIssuesAsync()
    {
        var response = await _session.Query<IssueResponseModel>().ToListAsync();
        return response;
    }
}
