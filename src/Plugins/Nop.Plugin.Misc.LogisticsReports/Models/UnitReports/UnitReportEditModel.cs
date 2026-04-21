using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Misc.LogisticsReports.Models.UnitReports;

public class UnitReportEditModel
{
    public int Id { get; set; }
    public int ReportPeriodId { get; set; }
    public string ReportPeriodName { get; set; } = string.Empty;
    public int OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public IFormFile? ImportFile { get; set; }
    public string ReturnReason { get; set; } = string.Empty;
    public IList<UnitReportValueModel> Values { get; set; } = new List<UnitReportValueModel>();
}
