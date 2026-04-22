using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers;

/// <summary>
/// Customer military profile service interface
/// </summary>
public partial interface ICustomerMilitaryProfileService
{
    /// <summary>
    /// Get a customer military profile by customer identifier
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer military profile
    /// </returns>
    Task<CustomerMilitaryProfile> GetByCustomerIdAsync(int customerId);

    /// <summary>
    /// Insert customer military profile
    /// </summary>
    /// <param name="profile">Customer military profile</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAsync(CustomerMilitaryProfile profile);

    /// <summary>
    /// Update customer military profile
    /// </summary>
    /// <param name="profile">Customer military profile</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAsync(CustomerMilitaryProfile profile);
}
