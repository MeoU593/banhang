using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Builders;

public class DocumentIssuerBuilder : NopEntityBuilder<DocumentIssuer>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DocumentIssuer.Name)).AsString(250).NotNullable()
            .WithColumn(nameof(DocumentIssuer.Description)).AsString(1000).Nullable()
            .WithColumn(nameof(DocumentIssuer.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0);
    }
}
