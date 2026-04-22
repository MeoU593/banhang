using Nop.Core.Domain.Customers;
using Nop.Data;

namespace Nop.Services.Customers;

/// <summary>
/// Customer military profile service
/// </summary>
public partial class CustomerMilitaryProfileService : ICustomerMilitaryProfileService
{
    #region Fields

    protected readonly IRepository<CustomerMilitaryProfile> _customerMilitaryProfileRepository;

    #endregion

    #region Ctor

    public CustomerMilitaryProfileService(IRepository<CustomerMilitaryProfile> customerMilitaryProfileRepository)
    {
        _customerMilitaryProfileRepository = customerMilitaryProfileRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get a customer military profile by customer identifier
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer military profile
    /// </returns>
    public virtual async Task<CustomerMilitaryProfile> GetByCustomerIdAsync(int customerId)
    {
        return await _customerMilitaryProfileRepository.Table
            .OrderByDescending(profile => profile.Id)
            .FirstOrDefaultAsync(profile => profile.CustomerId == customerId);
    }

    /// <summary>
    /// Insert customer military profile
    /// </summary>
    /// <param name="profile">Customer military profile</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertAsync(CustomerMilitaryProfile profile)
    {
        await _customerMilitaryProfileRepository.InsertAsync(profile);
    }

    /// <summary>
    /// Update customer military profile
    /// </summary>
    /// <param name="profile">Customer military profile</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateAsync(CustomerMilitaryProfile profile)
    {
        await _customerMilitaryProfileRepository.UpdateAsync(profile);
    }

    #endregion
}
