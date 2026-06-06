using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/26 11:00:00", "Add user guide topic", MigrationProcessType.Update)]
public class AddUserGuideTopicMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('Topic', 'U') IS NOT NULL
                AND OBJECT_ID('TopicTemplate', 'U') IS NOT NULL
            BEGIN
                DECLARE @UserGuideTitle NVARCHAR(200) = N'Tài liệu hướng dẫn sử dụng';
                DECLARE @UserGuideBody NVARCHAR(MAX) = N'
                    <div class=''user-guide-topic-content''>
                        <p><strong>Tài liệu hướng dẫn sử dụng</strong> giúp cán bộ, nhân viên và quản trị đơn vị thao tác nhanh trên cổng thông tin hậu cần - quân nhu.</p>
                        <h2>1. Đăng nhập và hồ sơ cá nhân</h2>
                        <ol><li>Đăng nhập bằng tài khoản được cấp.</li><li>Cập nhật họ tên, số điện thoại, cấp bậc, chức vụ và đơn vị công tác.</li></ol>
                        <h2>2. Sản phẩm, tin tức, văn bản và tài liệu</h2>
                        <ol><li>Dùng menu bên trái để mở từng phân hệ.</li><li>Sử dụng bộ lọc và ô tìm kiếm để tra cứu nhanh dữ liệu.</li></ol>
                        <h2>3. Quản trị đơn vị</h2>
                        <ol><li>Trưởng đơn vị cập nhật danh bạ, nhân sự và báo cáo theo phân quyền.</li><li>Quản trị hệ thống theo dõi toàn bộ đơn vị và nhật ký hệ thống.</li></ol>
                    </div>';

                DECLARE @DefaultTopicTemplateId INT =
                (
                    SELECT TOP 1 [Id]
                    FROM [TopicTemplate]
                    WHERE [Name] = N'Default template'
                    ORDER BY [Id]
                );

                IF @DefaultTopicTemplateId IS NULL
                BEGIN
                    SELECT TOP 1 @DefaultTopicTemplateId = [Id]
                    FROM [TopicTemplate]
                    ORDER BY [Id];
                END

                IF @DefaultTopicTemplateId IS NOT NULL
                    AND NOT EXISTS (SELECT 1 FROM [Topic] WHERE [SystemName] = N'UserGuide')
                BEGIN
                    INSERT INTO [Topic]
                    (
                        [SystemName], [IncludeInSitemap], [DisplayOrder], [AccessibleWhenStoreClosed],
                        [IsPasswordProtected], [Password], [Title], [Body], [Published], [TopicTemplateId],
                        [MetaKeywords], [MetaDescription], [MetaTitle], [SubjectToAcl], [LimitedToStores],
                        [AvailableStartDateTimeUtc], [AvailableEndDateTimeUtc]
                    )
                    VALUES
                    (
                        N'UserGuide', 0, 1, 0, 0, NULL, @UserGuideTitle, @UserGuideBody, 1,
                        @DefaultTopicTemplateId, NULL, NULL, NULL, 0, 0, NULL, NULL
                    );
                END
                ELSE
                BEGIN
                    UPDATE [Topic]
                    SET [Title] = @UserGuideTitle,
                        [Body] = @UserGuideBody,
                        [Published] = 1,
                        [TopicTemplateId] = CASE WHEN [TopicTemplateId] = 0 THEN @DefaultTopicTemplateId ELSE [TopicTemplateId] END
                    WHERE [SystemName] = N'UserGuide';
                END

                IF OBJECT_ID('UrlRecord', 'U') IS NOT NULL
                BEGIN
                    DECLARE @UserGuideTopicId INT = (SELECT TOP 1 [Id] FROM [Topic] WHERE [SystemName] = N'UserGuide' ORDER BY [Id]);

                    IF @UserGuideTopicId IS NOT NULL
                        AND NOT EXISTS
                        (
                            SELECT 1
                            FROM [UrlRecord]
                            WHERE [EntityId] = @UserGuideTopicId
                                AND [EntityName] = N'Topic'
                                AND [LanguageId] = 0
                                AND [IsActive] = 1
                        )
                    BEGIN
                        INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [IsActive], [LanguageId])
                        VALUES (@UserGuideTopicId, N'Topic', N'tai-lieu-huong-dan-su-dung', 1, 0);
                    END
                END
            END
        ");
    }
}
