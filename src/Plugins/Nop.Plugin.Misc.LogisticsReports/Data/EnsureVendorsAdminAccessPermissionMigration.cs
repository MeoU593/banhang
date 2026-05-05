using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/26 11:00:00", "Misc.LogisticsReports ensure Vendors role can access admin panel", MigrationProcessType.Update)]
public class EnsureVendorsAdminAccessPermissionMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var customerRoleRepository = EngineContext.Current.Resolve<IRepository<CustomerRole>>();
        var permissionRecordRepository = EngineContext.Current.Resolve<IRepository<PermissionRecord>>();
        var permissionMappingRepository = EngineContext.Current.Resolve<IRepository<PermissionRecordCustomerRoleMapping>>();

        var vendorsRole = customerRoleRepository.Table.FirstOrDefault(x => x.SystemName == NopCustomerDefaults.VendorsRoleName);
        if (vendorsRole == null)
            return;

        var adminAccessPermission = permissionRecordRepository.Table
            .FirstOrDefault(x => x.SystemName == StandardPermission.Security.ACCESS_ADMIN_PANEL);
        if (adminAccessPermission == null)
            return;

        var mappingExists = permissionMappingRepository.Table.Any(x =>
            x.CustomerRoleId == vendorsRole.Id &&
            x.PermissionRecordId == adminAccessPermission.Id);

        if (mappingExists)
            return;

        permissionMappingRepository.InsertAsync(new PermissionRecordCustomerRoleMapping
        {
            CustomerRoleId = vendorsRole.Id,
            PermissionRecordId = adminAccessPermission.Id
        }).GetAwaiter().GetResult();
    }

    public override void Down()
    {
    }
}
