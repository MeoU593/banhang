using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IOrganizationUnitService
{
    Task<OrganizationUnit> EnsureRootUnitAsync();
    Task<IList<OrganizationUnit>> GetAllAsync(bool onlyActive = false);
    Task<OrganizationUnit?> GetByIdAsync(int id);
    Task InsertAsync(OrganizationUnit entity);
    Task UpdateAsync(OrganizationUnit entity);
    Task DeleteAsync(OrganizationUnit entity);
    Task<IList<OrganizationUnit>> GetChildrenAsync(int? parentId);
    Task<IList<OrganizationUnit>> GetDescendantsAsync(int organizationUnitId);
    Task<string> BuildPathAsync(OrganizationUnit entity);
}
