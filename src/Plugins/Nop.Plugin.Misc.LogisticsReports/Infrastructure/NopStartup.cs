using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.LogisticsReports.Services;

namespace Nop.Plugin.Misc.LogisticsReports.Infrastructure;

public class NopStartup : INopStartup
{
    public int Order => 3000;

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStorageService, StorageService>();
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
        EnsureVendorsRolePermissions(application);
    }

    private static void EnsureVendorsRolePermissions(IApplicationBuilder application)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        using var scope = application.ApplicationServices.CreateScope();

        var customerRoleRepository = scope.ServiceProvider.GetRequiredService<IRepository<CustomerRole>>();
        var permissionRecordRepository = scope.ServiceProvider.GetRequiredService<IRepository<PermissionRecord>>();
        var permissionMappingRepository = scope.ServiceProvider.GetRequiredService<IRepository<PermissionRecordCustomerRoleMapping>>();

        VendorsPermissionBootstrap.EnsureVendorRolePermissions(
            customerRoleRepository,
            permissionRecordRepository,
            permissionMappingRepository);
    }
}
