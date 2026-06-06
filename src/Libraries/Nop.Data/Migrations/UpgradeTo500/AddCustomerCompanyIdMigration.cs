using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-23 10:00:00", "Add customer company registration column")]
public class AddCustomerCompanyIdMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var customerTableName = nameof(Customer);
        var companyIdColumnName = nameof(Customer.CompanyId);

        if (!Schema.Table(customerTableName).Column(companyIdColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(companyIdColumnName)
                .AsInt32()
                .Nullable()
                .SetExistingRowsTo(null);
        }
    }
}
