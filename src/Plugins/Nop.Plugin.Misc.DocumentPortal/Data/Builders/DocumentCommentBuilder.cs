using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Builders;

public class DocumentCommentBuilder : NopEntityBuilder<DocumentComment>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DocumentComment.DocumentId)).AsInt32().ForeignKey<Document>()
            .WithColumn(nameof(DocumentComment.CustomerId)).AsInt32().ForeignKey<Customer>()
            .WithColumn(nameof(DocumentComment.CommentText)).AsCustom("NVARCHAR(MAX)").NotNullable()
            .WithColumn(nameof(DocumentComment.IsApproved)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(DocumentComment.CreatedOnUtc)).AsDateTime2().NotNullable();
    }
}
