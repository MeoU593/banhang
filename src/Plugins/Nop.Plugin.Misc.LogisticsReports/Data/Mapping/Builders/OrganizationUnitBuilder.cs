using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.LogisticsReports.Domain;

namespace Nop.Plugin.Misc.LogisticsReports.Data.Mapping.Builders;

public class OrganizationUnitBuilder : NopEntityBuilder<OrganizationUnit>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OrganizationUnit.Code)).AsString(128).NotNullable()
            .WithColumn(nameof(OrganizationUnit.Name)).AsString(512).NotNullable()
            .WithColumn(nameof(OrganizationUnit.ParentId)).AsInt32().Nullable()
            .WithColumn(nameof(OrganizationUnit.Level)).AsInt32().NotNullable()
            .WithColumn(nameof(OrganizationUnit.Path)).AsString(2000).NotNullable()
            .WithColumn(nameof(OrganizationUnit.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(OrganizationUnit.IsActive)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(OrganizationUnit.CreatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(OrganizationUnit.UpdatedOnUtc)).AsDateTime2().NotNullable();
    }
}
