using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

/// <summary>
/// Disabled queued email service. Email remains a profile/contact field only.
/// </summary>
public partial class QueuedEmailService : IQueuedEmailService
{
    public virtual Task InsertQueuedEmailAsync(QueuedEmail queuedEmail)
    {
        return Task.CompletedTask;
    }

    public virtual Task UpdateQueuedEmailAsync(QueuedEmail queuedEmail)
    {
        return Task.CompletedTask;
    }

    public virtual Task DeleteQueuedEmailAsync(QueuedEmail queuedEmail)
    {
        return Task.CompletedTask;
    }

    public virtual Task DeleteQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
    {
        return Task.CompletedTask;
    }

    public virtual Task<QueuedEmail> GetQueuedEmailByIdAsync(int queuedEmailId)
    {
        return Task.FromResult<QueuedEmail>(null);
    }

    public virtual Task<IList<QueuedEmail>> GetQueuedEmailsByIdsAsync(int[] queuedEmailIds)
    {
        return Task.FromResult<IList<QueuedEmail>>(new List<QueuedEmail>());
    }

    public virtual Task RequeueQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
    {
        return Task.CompletedTask;
    }

    public virtual Task<IPagedList<QueuedEmail>> SearchEmailsAsync(string fromEmail,
        string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
        bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
        bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return Task.FromResult<IPagedList<QueuedEmail>>(new PagedList<QueuedEmail>(new List<QueuedEmail>(), pageIndex, pageSize));
    }

    public virtual Task<int> DeleteAlreadySentEmailsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
    {
        return Task.FromResult(0);
    }

    public virtual Task DeleteAllEmailsAsync()
    {
        return Task.CompletedTask;
    }
}
