using Nop.Data;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class ReportPeriodService : IReportPeriodService
{
    private readonly IRepository<ReportPeriod> _periodRepository;
    private readonly IRepository<UnitReport> _unitReportRepository;
    private readonly IRepository<Vendor> _vendorRepository;

    public ReportPeriodService(IRepository<ReportPeriod> periodRepository,
        IRepository<UnitReport> unitReportRepository,
        IRepository<Vendor> vendorRepository)
    {
        _periodRepository = periodRepository;
        _unitReportRepository = unitReportRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<IList<ReportPeriod>> GetAllAsync() => await _periodRepository.Table.OrderByDescending(x => x.FromDateUtc).ToListAsync();
    public async Task<ReportPeriod?> GetByIdAsync(int id) => await _periodRepository.GetByIdAsync(id);
    public async Task InsertAsync(ReportPeriod entity) => await _periodRepository.InsertAsync(entity);
    public async Task UpdateAsync(ReportPeriod entity) => await _periodRepository.UpdateAsync(entity);
    public async Task DeleteAsync(ReportPeriod entity) => await _periodRepository.DeleteAsync(entity);

    public async Task<int> EnsureUnitReportsCreatedAsync(int reportPeriodId)
    {
        var period = await GetByIdAsync(reportPeriodId) ?? throw new ArgumentException("Report period not found");
        var existingOrgIds = await _unitReportRepository.Table.Where(x => x.ReportPeriodId == reportPeriodId)
            .Select(x => x.OrganizationUnitId)
            .ToListAsync();

        var organizations = await _vendorRepository.Table.Where(x => !x.Deleted && x.Active).ToListAsync();
        var missing = organizations.Where(x => !existingOrgIds.Contains(x.Id)).ToList();
        if (!missing.Any())
            return 0;

        var unitReports = missing.Select(x => new UnitReport
        {
            ReportPeriodId = period.Id,
            OrganizationUnitId = x.Id,
            ParentOrganizationUnitId = x.ParentId,
            StatusId = (int)UnitReportStatus.Draft,
            CurrentVersion = 1
        }).ToList();

        await _unitReportRepository.InsertAsync(unitReports);
        return unitReports.Count;
    }
}
