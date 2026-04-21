using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class CustomerOrganizationUnitService : ICustomerOrganizationUnitService
{
    private readonly IRepository<CustomerOrganizationUnit> _mappingRepository;

    public CustomerOrganizationUnitService(IRepository<CustomerOrganizationUnit> mappingRepository)
    {
        _mappingRepository = mappingRepository;
    }

    public async Task<IList<CustomerOrganizationUnit>> GetAllAsync()
        => await _mappingRepository.Table.OrderBy(x => x.OrganizationUnitId).ThenBy(x => x.CustomerId).ToListAsync();

    public async Task<IList<CustomerOrganizationUnit>> GetByCustomerIdAsync(int customerId)
        => await _mappingRepository.Table.Where(x => x.CustomerId == customerId).ToListAsync();

    public async Task<IList<CustomerOrganizationUnit>> GetByOrganizationUnitIdAsync(int organizationUnitId)
        => await _mappingRepository.Table.Where(x => x.OrganizationUnitId == organizationUnitId).ToListAsync();

    public async Task<CustomerOrganizationUnit?> GetByIdAsync(int id)
        => await _mappingRepository.GetByIdAsync(id);

    public async Task InsertAsync(CustomerOrganizationUnit entity)
        => await _mappingRepository.InsertAsync(entity);

    public async Task UpdateAsync(CustomerOrganizationUnit entity)
        => await _mappingRepository.UpdateAsync(entity);

    public async Task DeleteAsync(CustomerOrganizationUnit entity)
        => await _mappingRepository.DeleteAsync(entity);
}
