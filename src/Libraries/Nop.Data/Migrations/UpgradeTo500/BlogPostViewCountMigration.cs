using FluentMigrator;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-04 09:00:00", "Add view count to blog post")]
public class BlogPostViewCountMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        var tableName = nameof(BlogPost);
        var viewCountColumnName = nameof(BlogPost.ViewCount);

        if (!Schema.Table(tableName).Column(viewCountColumnName).Exists())
        {
            Alter.Table(tableName)
                .AddColumn(viewCountColumnName)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }
    }
}
