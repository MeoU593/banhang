using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IReportTemplateService
{
    Task<IList<ReportTemplate>> GetAllAsync();
    Task<ReportTemplate?> GetByIdAsync(int id);
    Task InsertAsync(ReportTemplate entity);
    Task UpdateAsync(ReportTemplate entity);
    Task DeleteAsync(ReportTemplate entity);

    Task<IList<ReportIndicator>> GetIndicatorsAsync(int reportTemplateId);
    Task<ReportIndicator?> GetIndicatorByIdAsync(int id);
    Task InsertIndicatorAsync(ReportIndicator entity);
    Task UpdateIndicatorAsync(ReportIndicator entity);
    Task DeleteIndicatorAsync(ReportIndicator entity);
}
