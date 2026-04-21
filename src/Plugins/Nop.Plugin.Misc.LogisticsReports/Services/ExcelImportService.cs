using ClosedXML.Excel;
using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Plugin.Misc.LogisticsReports.Enums;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class ExcelImportService : IExcelImportService
{
    private readonly IRepository<UnitReport> _unitReportRepository;
    private readonly IRepository<UnitReportValue> _valueRepository;
    private readonly IRepository<ReportPeriod> _periodRepository;
    private readonly IRepository<ReportIndicator> _indicatorRepository;
    private readonly IStorageService _storageService;

    public ExcelImportService(
        IRepository<UnitReport> unitReportRepository,
        IRepository<UnitReportValue> valueRepository,
        IRepository<ReportPeriod> periodRepository,
        IRepository<ReportIndicator> indicatorRepository,
        IStorageService storageService)
    {
        _unitReportRepository = unitReportRepository;
        _valueRepository = valueRepository;
        _periodRepository = periodRepository;
        _indicatorRepository = indicatorRepository;
        _storageService = storageService;
    }

    public async Task<(int successCount, int errorCount, string errorLog)> ImportUnitReportAsync(int unitReportId, string storedFilePath, int customerId)
    {
        var unitReport = await _unitReportRepository.GetByIdAsync(unitReportId) ?? throw new ArgumentException("Không tìm thấy báo cáo đơn vị");
        var period = await _periodRepository.GetByIdAsync(unitReport.ReportPeriodId) ?? throw new ArgumentException("Report period not found");
        var indicators = await _indicatorRepository.Table
            .Where(x => x.ReportTemplateId == period.ReportTemplateId && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();

        var values = await _valueRepository.Table.Where(x => x.UnitReportId == unitReportId).ToListAsync();
        var valueMap = values.ToDictionary(x => x.ReportIndicatorId, x => x);
        var errors = new List<string>();
        var successCount = 0;

        using var stream = _storageService.OpenRead(storedFilePath);
        using var workbook = new XLWorkbook(stream);

        foreach (var indicator in indicators)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(indicator.ExcelSheetName) || string.IsNullOrWhiteSpace(indicator.ExcelCellAddress))
                    continue;

                var worksheet = workbook.Worksheet(indicator.ExcelSheetName);
                var cell = worksheet.Cell(indicator.ExcelCellAddress);
                if (!valueMap.TryGetValue(indicator.Id, out var target))
                    continue;

                target.LastUpdatedByCustomerId = customerId;
                target.LastUpdatedOnUtc = DateTime.UtcNow;
                target.SourceTypeId = (int)ValueSourceType.Imported;

                if (indicator.DataTypeId == (int)IndicatorDataType.Text)
                {
                    target.SelfValueText = cell.GetString();
                    target.FinalValueText = target.SelfValueText;
                }
                else
                {
                    if (!decimal.TryParse(cell.Value.ToString(), out var decimalValue))
                    {
                        if (indicator.IsRequired)
                            errors.Add($"{indicator.Code} - cell {indicator.ExcelSheetName}!{indicator.ExcelCellAddress}: invalid number");
                        continue;
                    }

                    target.SelfValueNumber = decimalValue;
                    target.FinalValueNumber = (target.SelfValueNumber ?? 0) + (target.ChildAggregateValueNumber ?? 0) + (target.AdjustmentValueNumber ?? 0);
                }

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"{indicator.Code}: {ex.Message}");
            }
        }

        if (valueMap.Values.Any())
            await _valueRepository.UpdateAsync(valueMap.Values.ToList());

        var errorLog = errors.Any()
            ? await _storageService.SaveTextAsync("Imports", $"import-errors-{unitReportId}.txt", string.Join(Environment.NewLine, errors))
            : string.Empty;

        return (successCount, errors.Count, errorLog);
    }
}
