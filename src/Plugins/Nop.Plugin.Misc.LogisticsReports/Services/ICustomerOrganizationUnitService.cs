using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface ICustomerOrganizationUnitService
{
    Task<IList<CustomerOrganizationUnit>> GetAllAsync();
    Task<IList<CustomerOrganizationUnit>> GetByCustomerIdAsync(int customerId);
    Task<IList<CustomerOrganizationUnit>> GetByOrganizationUnitIdAsync(int organizationUnitId);
    Task<CustomerOrganizationUnit?> GetByIdAsync(int id);
    Task InsertAsync(CustomerOrganizationUnit entity);
    Task UpdateAsync(CustomerOrganizationUnit entity);
    Task DeleteAsync(CustomerOrganizationUnit entity);
}
