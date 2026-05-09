using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

// Empty stub - newsletter features have been removed.
public partial class NoopNewsLetterSubscriptionService : INewsLetterSubscriptionService
{
    public virtual Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        return Task.CompletedTask;
    }

    public virtual Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        return Task.CompletedTask;
    }

    public virtual Task DeleteNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        return Task.CompletedTask;
    }

    public virtual Task<NewsLetterSubscription> GetNewsLetterSubscriptionByIdAsync(int newsLetterSubscriptionId)
    {
        return Task.FromResult<NewsLetterSubscription>(null);
    }

    public virtual Task<IList<NewsLetterSubscription>> GetNewsLetterSubscriptionsByGuidAsync(Guid newsLetterSubscriptionGuid)
    {
        return Task.FromResult<IList<NewsLetterSubscription>>(new List<NewsLetterSubscription>());
    }

    public virtual Task<IList<NewsLetterSubscription>> GetNewsLetterSubscriptionsByEmailAsync(string email,
        int storeId = 0, int subscriptionTypeId = 0, bool? isActive = null)
    {
        return Task.FromResult<IList<NewsLetterSubscription>>(new List<NewsLetterSubscription>());
    }

    public virtual Task<IPagedList<NewsLetterSubscription>> GetAllNewsLetterSubscriptionsAsync(string email = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int storeId = 0, bool? isActive = null, int customerRoleId = 0, int subscriptionTypeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return Task.FromResult<IPagedList<NewsLetterSubscription>>(new PagedList<NewsLetterSubscription>(new List<NewsLetterSubscription>(), pageIndex, pageSize));
    }
}
