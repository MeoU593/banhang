using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

// Empty stub - newsletter features have been removed.
public partial class NoopNewsLetterSubscriptionTypeService : INewsLetterSubscriptionTypeService
{
    public virtual Task InsertNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
        return Task.CompletedTask;
    }

    public virtual Task UpdateNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
        return Task.CompletedTask;
    }

    public virtual Task DeleteNewsLetterSubscriptionTypeAsync(NewsLetterSubscriptionType newsLetterSubscriptionType)
    {
        return Task.CompletedTask;
    }

    public virtual Task<NewsLetterSubscriptionType> GetNewsLetterSubscriptionTypeByIdAsync(int newsLetterSubscriptionTypeId)
    {
        return Task.FromResult<NewsLetterSubscriptionType>(null);
    }

    public virtual Task<IList<NewsLetterSubscriptionType>> GetAllNewsLetterSubscriptionTypesAsync(int storeId = 0)
    {
        return Task.FromResult<IList<NewsLetterSubscriptionType>>(new List<NewsLetterSubscriptionType>());
    }
}
