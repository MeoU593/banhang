using System.Collections.Generic;
using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-24 20:15:00", "5.00.2", UpdateMigrationType.Localization)]
public class SystemActivityLogLocalizationMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["ActivityLog.PublicStore.Login.Success"] = "Đăng nhập thành công",
            ["ActivityLog.PublicStore.Logout"] = "Đăng xuất",
            ["ActivityLog.AddNewNewsPost"] = "Đăng tin tức mới: {0}",
            ["ActivityLog.AddNewBlogDocument"] = "Đăng văn bản mới: {0}",
            ["ActivityLog.AddNewPortalDocument"] = "Đăng tài liệu mới: {0}",
            ["ActivityLog.AddNewProduct"] = "Đăng sản phẩm mới: {0}",
            ["ActivityLog.SubmitUnitReport"] = "Gửi báo cáo đơn vị: {0}"
        });
    }

    public override void Down()
    {
    }
}
