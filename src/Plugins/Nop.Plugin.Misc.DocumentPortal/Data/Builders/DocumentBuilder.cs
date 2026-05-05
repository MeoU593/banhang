using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.DocumentPortal.Domain;

namespace Nop.Plugin.Misc.DocumentPortal.Data.Builders;

public class DocumentBuilder : NopEntityBuilder<Document>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Document.Title)).AsString(500).NotNullable()
            .WithColumn(nameof(Document.Slug)).AsString(500).Nullable()
            .WithColumn(nameof(Document.Code)).AsString(100).Nullable()
            .WithColumn(nameof(Document.Summary)).AsString(1000).Nullable()
            .WithColumn(nameof(Document.Content)).AsCustom("NVARCHAR(MAX)").Nullable()
            .WithColumn(nameof(Document.Keywords)).AsString(1000).Nullable()
            .WithColumn(nameof(Document.DocumentCategoryId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.DocumentTypeId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.IssuerId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.IssuedDate)).AsDateTime2().Nullable()
            .WithColumn(nameof(Document.EffectiveDate)).AsDateTime2().Nullable()
            .WithColumn(nameof(Document.DownloadId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.ThumbnailPictureId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.Published)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(Document.ShowOnHomepage)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(Document.AllowDownload)).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(nameof(Document.ViewCount)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(Document.DownloadCount)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(Document.DisplayOrder)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(Document.Deleted)).AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(nameof(Document.UploadedByCustomerId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.OwnerVendorId)).AsInt32().Nullable()
            .WithColumn(nameof(Document.AccessScopeId)).AsInt32().NotNullable().WithDefaultValue((int)DocumentAccessScope.Public)
            .WithColumn(nameof(Document.CreatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(Document.UpdatedOnUtc)).AsDateTime2().NotNullable();
    }
}
