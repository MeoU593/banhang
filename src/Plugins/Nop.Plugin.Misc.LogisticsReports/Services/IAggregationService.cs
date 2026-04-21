namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IAggregationService
{
    Task AggregateForUnitReportAsync(int unitReportId, int customerId);
}
