using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.LogisticsReports.Services;

namespace Nop.Plugin.Misc.LogisticsReports.Infrastructure;

public class NopStartup : INopStartup
{
    public int Order => 3000;

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IOrganizationUnitService, OrganizationUnitService>();
        services.AddScoped<ICustomerOrganizationUnitService, CustomerOrganizationUnitService>();
        services.AddScoped<IReportTemplateService, ReportTemplateService>();
        services.AddScoped<IReportPeriodService, ReportPeriodService>();
        services.AddScoped<IUnitReportService, UnitReportService>();
        services.AddScoped<IAggregationService, AggregationService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IExportService, ExportService>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }
}
