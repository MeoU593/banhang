using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-20 11:00:00", "Add vendor hierarchy columns")]
public class VendorHierarchyMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var vendorTableName = nameof(Vendor);

        var codeColumnName = nameof(Vendor.Code);
        if (!Schema.Table(vendorTableName).Column(codeColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(codeColumnName)
                .AsString(128)
                .SetExistingRowsTo(null);
        }

        var parentIdColumnName = nameof(Vendor.ParentId);
        if (!Schema.Table(vendorTableName).Column(parentIdColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(parentIdColumnName)
                .AsInt32()
                .SetExistingRowsTo(null);
        }

        var levelColumnName = nameof(Vendor.Level);
        if (!Schema.Table(vendorTableName).Column(levelColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(levelColumnName)
                .AsInt32()
                .SetExistingRowsTo(0);
        }

        var pathColumnName = nameof(Vendor.Path);
        if (!Schema.Table(vendorTableName).Column(pathColumnName).Exists())
        {
            Alter.Table(vendorTableName)
                .AddColumn(pathColumnName)
                .AsString(2000)
                .SetExistingRowsTo(null);
        }
    }
}
