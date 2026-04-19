using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Builders;

public class DocumentCategoryBuilder : NopEntityBuilder<DocumentCategory>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DocumentCategory.Name)).AsString(250).NotNullable()
            .WithColumn(nameof(DocumentCategory.ParentCategoryId)).AsInt32().Nullable()
            .WithColumn(nameof(DocumentCategory.Description)).AsString(1000).Nullable()
            .WithColumn(nameof(DocumentCategory.Published)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(DocumentCategory.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0);
    }
}
