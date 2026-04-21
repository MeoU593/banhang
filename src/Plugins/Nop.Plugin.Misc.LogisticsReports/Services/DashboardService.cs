using Nop.Data;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IRepository<ReportTemplate> _templateRepository;
    private readonly IRepository<ReportPeriod> _periodRepository;
    private readonly IRepository<UnitReport> _unitReportRepository;

    public DashboardService(
        IRepository<Vendor> vendorRepository,
        IRepository<ReportTemplate> templateRepository,
        IRepository<ReportPeriod> periodRepository,
        IRepository<UnitReport> unitReportRepository)
    {
        _vendorRepository = vendorRepository;
        _templateRepository = templateRepository;
        _periodRepository = periodRepository;
        _unitReportRepository = unitReportRepository;
    }

    public async Task<DashboardSummaryModel> BuildSummaryAsync()
    {
        return new DashboardSummaryModel
        {
            TotalOrganizations = await _vendorRepository.Table.CountAsync(x => !x.Deleted),
            TotalTemplates = await _templateRepository.Table.CountAsync(),
            TotalPeriods = await _periodRepository.Table.CountAsync(),
            TotalUnitReports = await _unitReportRepository.Table.CountAsync(),
            SubmittedUnitReports = await _unitReportRepository.Table.CountAsync(x => x.StatusId == (int)UnitReportStatus.Submitted),
            ReturnedUnitReports = await _unitReportRepository.Table.CountAsync(x => x.StatusId == (int)UnitReportStatus.Returned),
            LockedUnitReports = await _unitReportRepository.Table.CountAsync(x => x.StatusId == (int)UnitReportStatus.Locked)
        };
    }
}
