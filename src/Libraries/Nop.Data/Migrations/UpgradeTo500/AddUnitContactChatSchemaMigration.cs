using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-05 10:00:00", "Add unit contact chat schema")]
public class AddUnitContactChatSchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(Vendor)).Column(nameof(Vendor.ContactCustomerId)).Exists())
        {
            Alter.Table(nameof(Vendor))
                .AddColumn(nameof(Vendor.ContactCustomerId))
                .AsInt32()
                .Nullable();
        }

        if (!Schema.Table(nameof(PrivateMessage)).Column(nameof(PrivateMessage.UnitContactVendorId)).Exists())
        {
            Alter.Table(nameof(PrivateMessage))
                .AddColumn(nameof(PrivateMessage.UnitContactVendorId))
                .AsInt32()
                .Nullable();
        }
    }
}
