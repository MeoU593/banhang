using Microsoft.AspNetCore.Http;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IUnitReportService
{
    Task<IList<UnitReport>> GetByPeriodAsync(int reportPeriodId);
    Task<IList<UnitReport>> GetByParentVendorAsync(int reportPeriodId, int parentVendorId);
    Task<UnitReport?> GetByIdAsync(int id);
    Task<IList<UnitReportValue>> GetValuesAsync(int unitReportId);
    Task SaveValuesAsync(int unitReportId, IList<UnitReportValue> values, int customerId);
    Task SubmitAsync(int unitReportId, int customerId);
    Task ReturnAsync(int unitReportId, int customerId, string reason);
    Task LockAsync(int unitReportId, int customerId);
    Task InitializeValuesIfNeededAsync(int unitReportId);
    Task<int> ImportAsync(int unitReportId, IFormFile file, int customerId);
}
