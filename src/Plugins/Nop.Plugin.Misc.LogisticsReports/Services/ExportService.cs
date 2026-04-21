using System.Text;
using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class ExportService : IExportService
{
    private readonly IRepository<UnitReportValue> _valueRepository;
    private readonly IRepository<ReportIndicator> _indicatorRepository;

    public ExportService(IRepository<UnitReportValue> valueRepository, IRepository<ReportIndicator> indicatorRepository)
    {
        _valueRepository = valueRepository;
        _indicatorRepository = indicatorRepository;
    }

    public async Task<byte[]> ExportUnitReportCsvAsync(int unitReportId)
    {
        var values = await _valueRepository.Table.Where(x => x.UnitReportId == unitReportId).ToListAsync();
        if (!values.Any())
            return Encoding.UTF8.GetBytes("IndicatorCode,IndicatorName,SelfValue,ChildAggregate,Adjustment,FinalValue,Note" + Environment.NewLine);

        var indicatorIds = values.Select(x => x.ReportIndicatorId).Distinct().ToList();
        var indicators = await _indicatorRepository.Table.Where(x => indicatorIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x);

        var sb = new StringBuilder();
        sb.AppendLine("IndicatorCode,IndicatorName,SelfValue,ChildAggregate,Adjustment,FinalValue,Note");
        foreach (var value in values.OrderBy(x => indicators.TryGetValue(x.ReportIndicatorId, out var indicator) ? indicator.DisplayOrder : int.MaxValue))
        {
            static string Escape(string? value) => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
            indicators.TryGetValue(value.ReportIndicatorId, out var indicator);

            sb.AppendLine(string.Join(",", new[]
            {
                Escape(indicator?.Code),
                Escape(indicator?.Name),
                Escape(value.SelfValueNumber?.ToString()),
                Escape(value.ChildAggregateValueNumber?.ToString()),
                Escape(value.AdjustmentValueNumber?.ToString()),
                Escape(value.FinalValueNumber?.ToString()),
                Escape(value.Note)
            }));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
