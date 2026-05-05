using Nop.Services.ScheduleTasks;

namespace Nop.Services.Forums;

/// <summary>
/// Deletes public unit contact chat messages after their retention period.
/// </summary>
public partial class DeleteUnitContactPrivateMessagesTask : IScheduleTask
{
    protected readonly IForumService _forumService;

    public DeleteUnitContactPrivateMessagesTask(IForumService forumService)
    {
        _forumService = forumService;
    }

    public virtual async Task ExecuteAsync()
    {
        await _forumService.DeleteUnitContactPrivateMessagesOlderThanAsync(DateTime.UtcNow.AddDays(-30));
    }
}
