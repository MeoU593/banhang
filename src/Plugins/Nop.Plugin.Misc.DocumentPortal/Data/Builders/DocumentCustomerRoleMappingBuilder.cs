using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Builders;

public class DocumentCustomerRoleMappingBuilder : NopEntityBuilder<DocumentCustomerRoleMapping>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DocumentCustomerRoleMapping.DocumentId)).AsInt32().NotNullable()
            .WithColumn(nameof(DocumentCustomerRoleMapping.CustomerRoleId)).AsInt32().NotNullable();
    }
}
