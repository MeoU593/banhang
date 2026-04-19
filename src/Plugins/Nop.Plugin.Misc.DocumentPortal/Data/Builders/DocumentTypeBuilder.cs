using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Builders;

public class DocumentTypeBuilder : NopEntityBuilder<DocumentType>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DocumentType.Name)).AsString(250).NotNullable()
            .WithColumn(nameof(DocumentType.Description)).AsString(1000).Nullable()
            .WithColumn(nameof(DocumentType.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0);
    }
}
