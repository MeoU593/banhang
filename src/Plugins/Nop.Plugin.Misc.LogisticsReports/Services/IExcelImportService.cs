namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IExcelImportService
{
    Task<(int successCount, int errorCount, string errorLog)> ImportUnitReportAsync(int unitReportId, string storedFilePath, int customerId);
}
