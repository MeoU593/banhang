using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class CustomerOrganizationUnitBuilder : NopEntityBuilder<CustomerOrganizationUnit>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerOrganizationUnit.CustomerId)).AsInt32().NotNullable()
            .WithColumn(nameof(CustomerOrganizationUnit.OrganizationUnitId)).AsInt32().NotNullable()
            .WithColumn(nameof(CustomerOrganizationUnit.IsPrimary)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(CustomerOrganizationUnit.CanInputReport)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(CustomerOrganizationUnit.CanReviewChildReports)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(CustomerOrganizationUnit.CanSubmitReport)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(CustomerOrganizationUnit.CanLockReport)).AsBoolean().NotNullable().WithDefaultValue(false);
    }
}
