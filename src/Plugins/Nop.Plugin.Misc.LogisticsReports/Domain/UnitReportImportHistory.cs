using Nop.Core;

namespace Nop.Plugin.Misc.LogisticsReports.Domain;

public class UnitReportImportHistory : BaseEntity
{
    public int UnitReportId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredFilePath { get; set; } = string.Empty;
    public int ImportedByCustomerId { get; set; }
    public DateTime ImportedOnUtc { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public string ErrorLogPath { get; set; } = string.Empty;
}
