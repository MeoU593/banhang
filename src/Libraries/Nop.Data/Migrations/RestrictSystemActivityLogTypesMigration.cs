using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/24 20:10:00", "Restrict system activity log types", MigrationProcessType.Update)]
public class RestrictSystemActivityLogTypesMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('ActivityLogType', 'U') IS NOT NULL
            BEGIN
                DECLARE @AllowedActivityTypes TABLE
                (
                    [SystemKeyword] NVARCHAR(100) NOT NULL,
                    [Name] NVARCHAR(200) NOT NULL
                );

                INSERT INTO @AllowedActivityTypes ([SystemKeyword], [Name])
                VALUES
                    ('PublicStore.SuccessfulLogin', N'Đăng nhập'),
                    ('PublicStore.Logout', N'Đăng xuất'),
                    ('AddNewNewsPost', N'Đăng tin tức'),
                    ('AddNewBlogDocument', N'Đăng văn bản'),
                    ('AddNewPortalDocument', N'Đăng tài liệu'),
                    ('AddNewProduct', N'Đăng sản phẩm'),
                    ('SubmitUnitReport', N'Đăng báo cáo');

                UPDATE [ActivityLogType]
                SET [Enabled] = 0
                WHERE [SystemKeyword] NOT IN (SELECT [SystemKeyword] FROM @AllowedActivityTypes);

                UPDATE alt
                SET alt.[Enabled] = 1,
                    alt.[Name] = allowed.[Name]
                FROM [ActivityLogType] alt
                INNER JOIN @AllowedActivityTypes allowed ON allowed.[SystemKeyword] = alt.[SystemKeyword];

                INSERT INTO [ActivityLogType] ([SystemKeyword], [Enabled], [Name])
                SELECT allowed.[SystemKeyword], 1, allowed.[Name]
                FROM @AllowedActivityTypes allowed
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM [ActivityLogType] alt
                    WHERE alt.[SystemKeyword] = allowed.[SystemKeyword]
                );
            END
        ");
    }
}
