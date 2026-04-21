using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IReportPeriodService
{
    Task<IList<ReportPeriod>> GetAllAsync();
    Task<ReportPeriod?> GetByIdAsync(int id);
    Task InsertAsync(ReportPeriod entity);
    Task UpdateAsync(ReportPeriod entity);
    Task DeleteAsync(ReportPeriod entity);
    Task<int> EnsureUnitReportsCreatedAsync(int reportPeriodId);
}
