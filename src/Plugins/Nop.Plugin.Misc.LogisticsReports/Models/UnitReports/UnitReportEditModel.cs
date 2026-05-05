using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;

public class UnitReportEditModel
{
    public int Id { get; set; }
    public int ReportPeriodId { get; set; }
    public string ReportPeriodName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool CanImport { get; set; }
    public bool CanSave { get; set; }
    public bool CanSubmit { get; set; }
    public bool CanAggregate { get; set; }
    public bool CanReturn { get; set; }
    public bool CanLock { get; set; }
    public IFormFile? ImportFile { get; set; }
    public string ReturnReason { get; set; } = string.Empty;
    public IList<UnitReportValueModel> Values { get; set; } = new List<UnitReportValueModel>();
}
