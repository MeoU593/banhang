using Nop.Data.Mapping;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping;

public class LogisticsReportsNameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        [typeof(OrganizationUnit)] = "LR_OrganizationUnit",
        [typeof(CustomerOrganizationUnit)] = "LR_CustomerOrganizationUnit",
        [typeof(ReportTemplate)] = "LR_ReportTemplate",
        [typeof(ReportIndicator)] = "LR_ReportIndicator",
        [typeof(ReportPeriod)] = "LR_ReportPeriod",
        [typeof(UnitReport)] = "LR_UnitReport",
        [typeof(UnitReportValue)] = "LR_UnitReportValue",
        [typeof(UnitReportImportHistory)] = "LR_UnitReportImportHistory",
        [typeof(UnitReportWorkflowLog)] = "LR_UnitReportWorkflowLog"
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}
