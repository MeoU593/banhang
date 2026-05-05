using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.LogisticsReports.Infrastructure;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/26 21:00:00", "Misc.LogisticsReports ensure Vendors role has operational permissions", MigrationProcessType.Update)]
public class EnsureVendorsOperationalPermissionsMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var customerRoleRepository = EngineContext.Current.Resolve<IRepository<CustomerRole>>();
        var permissionRecordRepository = EngineContext.Current.Resolve<IRepository<PermissionRecord>>();
        var permissionMappingRepository = EngineContext.Current.Resolve<IRepository<PermissionRecordCustomerRoleMapping>>();

        VendorsPermissionBootstrap.EnsureVendorRolePermissions(
            customerRoleRepository,
            permissionRecordRepository,
            permissionMappingRepository);
    }

    public override void Down()
    {
    }
}
