using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class ReportTemplateService : IReportTemplateService
{
    private readonly IRepository<ReportTemplate> _templateRepository;
    private readonly IRepository<ReportIndicator> _indicatorRepository;

    public ReportTemplateService(IRepository<ReportTemplate> templateRepository, IRepository<ReportIndicator> indicatorRepository)
    {
        _templateRepository = templateRepository;
        _indicatorRepository = indicatorRepository;
    }

    public async Task<IList<ReportTemplate>> GetAllAsync() => await _templateRepository.Table.OrderBy(x => x.Name).ToListAsync();
    public async Task<ReportTemplate?> GetByIdAsync(int id) => await _templateRepository.GetByIdAsync(id);
    public async Task InsertAsync(ReportTemplate entity) => await _templateRepository.InsertAsync(entity);
    public async Task UpdateAsync(ReportTemplate entity) => await _templateRepository.UpdateAsync(entity);
    public async Task DeleteAsync(ReportTemplate entity) => await _templateRepository.DeleteAsync(entity);

    public async Task<IList<ReportIndicator>> GetIndicatorsAsync(int reportTemplateId)
        => await _indicatorRepository.Table.Where(x => x.ReportTemplateId == reportTemplateId).OrderBy(x => x.DisplayOrder).ToListAsync();

    public async Task<ReportIndicator?> GetIndicatorByIdAsync(int id) => await _indicatorRepository.GetByIdAsync(id);
    public async Task InsertIndicatorAsync(ReportIndicator entity) => await _indicatorRepository.InsertAsync(entity);
    public async Task UpdateIndicatorAsync(ReportIndicator entity) => await _indicatorRepository.UpdateAsync(entity);
    public async Task DeleteIndicatorAsync(ReportIndicator entity) => await _indicatorRepository.DeleteAsync(entity);
}
