using FluentMigrator;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-04-22 14:00:00", "Add media, author and document downloads to blog post")]
public class BlogPostMediaAndAuthorMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var tableName = nameof(BlogPost);

        var thumbnailPictureIdColumnName = nameof(BlogPost.ThumbnailPictureId);
        if (!Schema.Table(tableName).Column(thumbnailPictureIdColumnName).Exists())
        {
            Alter.Table(tableName)
                .AddColumn(thumbnailPictureIdColumnName)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        var authorNameColumnName = nameof(BlogPost.AuthorName);
        if (!Schema.Table(tableName).Column(authorNameColumnName).Exists())
        {
            Alter.Table(tableName)
                .AddColumn(authorNameColumnName)
                .AsString(400)
                .SetExistingRowsTo(null);
        }

        var pdfDownloadIdColumnName = nameof(BlogPost.PdfDownloadId);
        if (!Schema.Table(tableName).Column(pdfDownloadIdColumnName).Exists())
        {
            Alter.Table(tableName)
                .AddColumn(pdfDownloadIdColumnName)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        var docDownloadIdColumnName = nameof(BlogPost.DocDownloadId);
        if (!Schema.Table(tableName).Column(docDownloadIdColumnName).Exists())
        {
            Alter.Table(tableName)
                .AddColumn(docDownloadIdColumnName)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }
    }
}
