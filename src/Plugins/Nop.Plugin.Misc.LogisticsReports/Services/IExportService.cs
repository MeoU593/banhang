namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IExportService
{
    Task<byte[]> ExportUnitReportCsvAsync(int unitReportId);
}
